using Machine.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
#nullable enable
    public static class OS
    {
        public static bool IsActive { get; private set; }
        internal static int ProcessCount { get; private set; } = 8;
        internal static int CommandsCount { get; private set; } = 8;
        internal static int PageTableSize { get; private set; } = 32;
        public static int DelayTime { get; private set; } = 1000;

        private static List<bool> RunningProcesses = new List<bool>(ProcessCount);

        public static async Task Run(int processCount, int commandsCount, int pageTableSize, int delayTime)
        {
            ProcessCount = processCount;
            CommandsCount = commandsCount;
            PageTableSize = pageTableSize;
            DelayTime = delayTime;

            PageTable.LoadPages(PageTableSize);

            for (int pid = 0; pid < ProcessCount; pid++)
            {
                RunningProcesses[pid] = true;
            }

            CommandsGenerator generator = new CommandsGenerator();
            IReadOnlyList<Command> commands = generator.Generate();
            await MMU.Run(commands);
        }
        internal async static Task<bool> SavePageChangesToDisk(Page page)
        {
            if (page != null)
            {
                if (page.IsDirty)
                {
                    page.IsDirty = false;
                    await SimulateHandling();
                    Counter.IncrementDiskAccesses(); //1 Disk access               
                }

                return true;
            }

            return false;
        }

        //todo: should we implement a page table for each process?
        internal static async Task<bool> LoadPage(Page page)
        {
            await Task.Delay(DelayTime);
            return true;
        }

        internal static void KillProcess(int processId)
        {
            RunningProcesses[processId] = false;
        }

        internal static async Task SimulateHandling()
        {
            IsActive = false;
            await Task.Delay(DelayTime);
            IsActive = true;
        }

        public static IReadOnlyList<bool> GetRunningProcesses()
        {
            return RunningProcesses.AsReadOnly();
        }
    }
}
