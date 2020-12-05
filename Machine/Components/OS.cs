﻿using Machine.Components;
using Machine.Utilities;
using System;
using System.Collections.Generic;
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
        /// The "sleep" time we use to simulate the "idle" state of the OS between operations.
        /// The value represents the number of milliseconds the task will be delayed / set on sleep.
        /// </summary>
        internal static int BetweenOpsDelayTime { get; private set; }
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

        public static event EventHandler OsStateChanged;

        /// <summary>
        /// The initializing method of the OS. Can be called from outside the project to start the simulation.
        /// </summary>
        /// <param name="processCount">The number of processes to be run during the simulation.</param>
        /// <param name="commandsCount">The number of commands to be executed during the simulation.</param>
        /// <param name="ramFrames">The number of frames the RAM will be divided into. Initially all are free.</param>
        /// <param name="maxPagesPerProcess">The maximum number of pages the page table of a process can hold.</param>
        /// <param name="delayTime">The value in milliseconds used for simulating the OS handling data-heavy operations.</param>
        public static async Task Run(int processCount = 8, int commandsCount = 48, int ramFrames = 8, int maxPagesPerProcess = 8, int delayTime = 1000, int betweenOpsDelay = 750)
        {
            IsActive = true;
            OsStateChanged?.Invoke(OsState.Idle, new EventArgs());
            FreeRamFrames = ramFrames;
            TotalRamCapacity = ramFrames;
            DelayTime = delayTime;
            BetweenOpsDelayTime = betweenOpsDelay;
            RamFramesTable = new List<RamFrame>();

            Counter.ResetCounter();
            Generator generator = new Generator();
            Processes = generator.GenerateProcesses(processCount, maxPagesPerProcess);
            RamFramesTable = generator.GenerateRamFrames(TotalRamCapacity);
            Commands = generator.GenerateCommands(commandsCount, processCount, Processes);
            await MMU.Run(Commands, Processes.AsReadOnly(), BetweenOpsDelayTime);           
            OsStateChanged?.Invoke(OsState.Free, new EventArgs());
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
            if (page.Requested != -1)
            {
                if (FreeRamFrames > 0)
                {
                    LoadRamFrame(page, TotalRamCapacity - FreeRamFrames);
                    FreeRamFrames--;
                    RamFramesChanged?.Invoke(null, new EventArgs());
                }
                else
                {
                    await SwapPage(page); 
                }
                
                page.Requested = -1;
                page.IsValid = true;
                Counter.IncrementDiskAccesses();
                Counter.IncrementPageFaults();
                await OS.SimulateHandling();
            }
        }

        /// <summary>
        /// The simulation of data-heavy operations performed by OS, such as moving data between memories.
        /// Sets the OS on inactive, sleeps for the preset delay time, then sets the OS back to active.
        /// </summary>
        internal static async Task SimulateHandling()
        {
            IsActive = false;
            OsStateChanged?.Invoke(OsState.Busy, new EventArgs());
            await Task.Delay(DelayTime);
            IsActive = true;
            OsStateChanged?.Invoke(OsState.Idle, new EventArgs());
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
            (Page pageToSwap, int pid) = GetPageToBeSwapped();

            if (pageToSwap.IsDirty)
            {
                await SavePageChangesToDisk(pageToSwap);
                pageToSwap.IsDirty = false;
            }

            pageToSwap.IsValid = false;
            pageToSwap.LastTimeAccessed = CurrentTimeGetter.GetCrtTime();
            LoadRamFrame(swapPage, FindIndexInRam(pid, pageToSwap.PageIndex));
            Counter.IncrementPageSwaps();
        }

        private static void LoadRamFrame(Page frameInfo, int frameIndex)
        {
            RamFramesTable[frameIndex].LastAccess = frameInfo.LastTimeAccessed;
            RamFramesTable[frameIndex].ProcessId = frameInfo.Requested;
            RamFramesTable[frameIndex].PtIndex = frameInfo.PageIndex;
        }

        internal static void OnCommandFinished(Command cmd)
        {
            Counter.IncrementRamAccesses(); //1 RAM access 
            CommandFinished?.Invoke(cmd, new EventArgs());
        }

        private static int FindIndexInRam(int pid, int pageIndex)
        {
            return RamFramesTable.IndexOf(RamFramesTable
                .Find(p => p.ProcessId == pid && p.PtIndex == pageIndex));
        }

        private static (Page PageToSwap, int Pid) GetPageToBeSwapped()
        {
            string lastAccess = CurrentTimeGetter.GetCrtTime();
            Page pageToSwap = new Page(-1);
            int pid = -1;

            foreach (var process in Processes)
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

            return (pageToSwap, pid);
        }

    }
}
