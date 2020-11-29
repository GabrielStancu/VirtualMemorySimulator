using Machine.Components;
using Machine.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    public static class OS
    {
        public static bool IsActive { get; private set; }
        internal static int ProcessCount { get; private set; }
        internal static int CommandsCount { get; private set; }
        internal static int MaxPagesPerProcess { get; private set; }
        internal static int RamFrames { get; private set; }
        internal static int DelayTime { get; private set; }

        internal static List<Process> Processes;
        public static async Task Run(int processCount = 8, int commandsCount = 48, int ramFrames = 32, int maxPagesPerProcess = 8, int delayTime = 1000)
        {
            IsActive = true;
            ProcessCount = processCount;
            CommandsCount = commandsCount;
            RamFrames = ramFrames;
            MaxPagesPerProcess = maxPagesPerProcess;
            DelayTime = delayTime;

            Generator generator = new Generator();
            Processes = generator.GenerateProcesses();

            IReadOnlyList<Command> commands = generator.GenerateCommands();
            await MMU.Run(commands);
            IsActive = false;
        }
        internal async static Task SavePageChangesToDisk(Page page)
        {
            if (page.IsDirty)
            {
                page.IsDirty = false;
                await SimulateHandling();
                Counter.IncrementDiskAccesses(); //1 Disk access               
            }
        }

        internal static async Task LoadPage()
        {
            foreach (var process in Processes)
            {
                foreach (var page in process.PageTable.Pages)
                {
                    if(page.Requested != 0)
                    {
                        if(RamFrames > 0)
                        {
                            RamFrames--;
                        }
                        else
                        {
                            await SwapPage();
                        }

                        await OS.SimulateHandling();

                        page.Requested = 0;
                        page.IsValid = true;
                        page.LastTimeAccessed = DateTime.Now;
                    }
                }
            }
        }

        internal static async Task SimulateHandling()
        {
            IsActive = false;
            await Task.Delay(DelayTime);
            IsActive = true;
        }

        public static IReadOnlyList<Process> GetRunningProcesses()
        {
            return Processes.AsReadOnly();
        }

        private async static Task SwapPage()
        {
            DateTime lastAccess = DateTime.Now;
            Page pageToSwap = new Page(-1);

            foreach(var process in Processes)
            {
                foreach (var page in process.PageTable.Pages)
                {
                    if (page.LastTimeAccessed < lastAccess && page.IsValid)
                    {
                        pageToSwap = page;
                        lastAccess = page.LastTimeAccessed;
                    }
                }
            }

            if(pageToSwap.IsDirty)
            {
                await SavePageChangesToDisk(pageToSwap);
                pageToSwap.IsDirty = false;
            }

            pageToSwap.IsValid = false;  
        }
    }
}
