using Machine.Components;
using Machine.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Machine
{
    /// <summary>
    /// Static class representing the operating system of our simulation unit.
    /// Manages the data transfer between main and secondary memory.
    /// </summary>
    public static class OS
    {
        /// <summary>
        /// True if the OS is not busy completing a task (e.g. moving data between memories),
        /// False otherwise.
        /// </summary>
        public static bool IsActive { get; private set; }
        /// <summary>
        /// The total number of processes to be run during the simulation.
        /// </summary>
        internal static int ProcessCount { get; private set; }
        /// <summary>
        /// The total number of commands to be run during the simulation.
        /// </summary>
        internal static int CommandsCount { get; private set; }
        /// <summary>
        /// The maximum number of pages a process can have.
        /// </summary>
        internal static int MaxPagesPerProcess { get; private set; }
        /// <summary>
        /// Holds the number of remaining unloaded frames.
        /// </summary>
        public static int FreeRamFrames { get; private set; }
        /// <summary>
        /// The number of frames we can divide the RAM.
        /// </summary>
        public static int TotalRamCapacity { get; private set; }
        /// <summary>
        /// The "sleep" time we use to simulate the moving of data between memories and similar tasks.
        /// The value represents the number of milliseconds the task will be delayed / set on sleep.
        /// </summary>
        internal static int DelayTime { get; private set; }
        /// <summary>
        /// The list of running processes in the simulation environment.
        /// </summary>
        internal static List<Process> Processes;

        /// <summary>
        /// The list of commands to be executed during the simulation.
        /// </summary>
        internal static IReadOnlyList<Command> Commands;

        public static List<RamFrame> RamFramesTable;

        public static event EventHandler RamFramesChanged;

        public static event EventHandler CommandFinished;

        //public static void InitCounter()
        //{
        //    Counter = new Counter();
        //}
        /// <summary>
        /// The initializing method of the OS. Can be called from outside the project to start the simulation.
        /// </summary>
        /// <param name="processCount">The number of processes to be run during the simulation.</param>
        /// <param name="commandsCount">The number of commands to be executed during the simulation.</param>
        /// <param name="ramFrames">The number of frames the RAM will be divided into. Initially all are free.</param>
        /// <param name="maxPagesPerProcess">The maximum number of pages the page table of a process can hold.</param>
        /// <param name="delayTime">The value in milliseconds used for simulating the OS handling data-heavy operations.</param>
        public static async Task Run(int processCount = 8, int commandsCount = 16, int ramFrames = 8, int maxPagesPerProcess = 8, int delayTime = 1)
        {
            IsActive = true;
            ProcessCount = processCount;
            CommandsCount = commandsCount;
            FreeRamFrames = ramFrames;
            TotalRamCapacity = ramFrames;
            MaxPagesPerProcess = maxPagesPerProcess;
            DelayTime = delayTime;
            RamFramesTable = new List<RamFrame>();

            Counter.ResetCounter();
            Generator generator = new Generator();
            Processes = generator.GenerateProcesses();
            RamFramesTable = generator.GenerateRamFrames(TotalRamCapacity);
            Commands = generator.GenerateCommands();
            await MMU.Run(Commands);
            IsActive = false;
        }

        /// <summary>
        /// Simulates asynchronously the process of saving data from RAM to Disk when the page is dirty.
        /// </summary>
        /// <param name="page">The page whose content will be written back to the Disk.</param>
        internal async static Task SavePageChangesToDisk(Page page)
        {
            if (page.IsDirty)
            {
                page.IsDirty = false;
                await SimulateHandling();
                Counter.IncrementDiskAccesses(); //1 Disk access               
            }
        }

        /// <summary>
        ///  Simulates asynchronously the process of loading a page from Disk to RAM, when requested.
        /// </summary>
        /// <param name="page">The page that's requested to be loaded to the RAM.</param>
        internal static async Task LoadPage(Page page)
        {
            if(page.Requested != -1)
            {
                page.LastTimeAccessed = DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);

                if (FreeRamFrames > 0)
                {                   
                    LoadRamFrame(page, TotalRamCapacity - FreeRamFrames);
                    FreeRamFrames--;
                    RamFramesChanged?.Invoke(null, new EventArgs());
                }
                else
                {
                    await SwapPage(page); //send ram frame through event here
                }

                await OS.SimulateHandling();

                page.Requested = -1;
                page.IsValid = true;
                

                Counter.IncrementDiskAccesses();
            }
        }

        /// <summary>
        /// The simulation of data-heavy operations performed by OS, such as moving data between memories.
        /// Sets the OS on inactive, sleeps for the preset delay time, then sets the OS back to active.
        /// </summary>
        internal static async Task SimulateHandling()
        {
            IsActive = false;
            await Task.Delay(DelayTime);
            IsActive = true;
        }

        /// <summary>
        /// Returns data about the processes running in the environment.
        /// </summary>
        /// <returns>A read-only list of Process objects, containing information about the running programs in the system.</returns>
        public static IReadOnlyList<Process> GetRunningProcesses()
            => Processes.AsReadOnly();

        /// <summary>
        /// Returns data about the commands run during the simulation.
        /// </summary>
        /// <returns>A read-only list of Command objects, containing information about the commands executed during the simulation.</returns>
        public static IReadOnlyList<Command> GetCommands()
            => Commands;

        public static IReadOnlyList<RamFrame> GetRamFrames()
            => RamFramesTable.AsReadOnly();

        /// <summary>
        /// Simulates asynchronously the process of swapping a page when the RAM is fully loaded and another page needs to be loaded.
        /// </summary>
        private async static Task SwapPage(Page swapPage)
        {
            string lastAccess = DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            Page pageToSwap = new Page(-1);
            int swapIndex, pid = -1;

            foreach(var process in Processes)
            {
                foreach (var page in process.PageTable.Pages)
                {
                    if (page.IsValid)
                    {
                        if(page.LastTimeAccessed.CompareTo(lastAccess) < 0)
                        {
                            pageToSwap = page;
                            lastAccess = page.LastTimeAccessed;
                            pid = process.Pid;
                        }
                    }
                }
            }

            if(pageToSwap.IsDirty)
            {
                await SavePageChangesToDisk(pageToSwap);
                pageToSwap.IsDirty = false;
            }

            pageToSwap.IsValid = false;
            swapIndex = FindIndexInRam(pid, pageToSwap.PageIndex);
            LoadRamFrame(swapPage, swapIndex);
            Counter.IncrementPageSwaps();
        }

        private static void LoadRamFrame(Page frameInfo, int frameIndex)
        {
            string crtTime = DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            frameInfo.LastTimeAccessed = crtTime;
            RamFramesTable[frameIndex].LastAccess = crtTime;
            RamFramesTable[frameIndex].ProcessId = frameInfo.Requested;
            RamFramesTable[frameIndex].PtIndex = frameInfo.PageIndex;
        }

        internal static void OnCommandFinished(Command cmd)
        {
            CommandFinished?.Invoke(cmd, new EventArgs());
        }

        private static int FindIndexInRam(int pid, int pageIndex)
        {
            return RamFramesTable.IndexOf(RamFramesTable.Find(p => p.ProcessId == pid && p.PtIndex == pageIndex));
        }
    }
}
