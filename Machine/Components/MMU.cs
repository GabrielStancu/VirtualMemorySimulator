using Machine.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Machine
{
    /// <summary>
    /// Class used to simulate the memory management unit (MMU).
    /// Performs accesses on the page table and counts the accesses of memory (RAM and Disk).
    /// </summary>
    internal static class MMU
    {
        /// <summary>
        /// Asynchronous method that runs each command on a separate Task / thread.
        /// </summary>
        /// <param name="commands">The commands to be run during the simulation.</param>
        internal async static Task Run(IReadOnlyList<Command> commands)
        {
            for (int index = 0; index < commands.Count; index++)
            {
                await AccessPage(commands[index]);
                commands[index].Completed = true;
                OS.OnCommandFinished(commands[index]);
                await Task.Delay(OS.BetweenOpsDelayTime);
            }
        }

        /// <summary>
        /// Method used by MMU to simulate the access of a page.
        /// First it checks whether the page exists in RAM and it belongs to the calling process, 
        /// then it checks whether the page is loaded. If not, the page is loaded then the read / write command is executed. 
        /// </summary>
        /// <param name="command">The command to be processed by the MMU.</param>
        private async static Task AccessPage(Command command)
        {
            Page page = OS.Processes[command.ProcessId].PageTable.GetPageByIndex(command.PageIndex);
            Counter.IncrementRamAccesses(); //1 RAM access 

            if (!page.IsValid)
            {
                await HandlePageRequested(command, page);
            }
 
            await HandleReadWriteCommand(command, page);
        }

        /// <summary>
        /// Method that handles the load of a page from disk to RAM. 
        /// Sets the Requested field of the page with the process' pid and calls the OS to load it.
        /// After the OS loaded it, the MMU is notified and resets the Requested field.
        /// The access time is updated, to maintain the priority queue for page swapping.
        /// </summary>
        /// <param name="command">Represents the received command (read / write).</param>
        /// <param name="page">The page for which the request was made.</param>
        private async static Task HandlePageRequested(Command command, Page page)
        {
            page.Requested = command.ProcessId;
            Counter.IncrementPageFaults();
            await OS.LoadPage(page);
        }

        /// <summary>
        /// Method that handles the actual read / write command.
        /// If the command is write, it simulates the handling by asking the OS to save changes to disk (if page is dirty)
        /// and sets the dirty bit and simulates the writing by the sleep unit of OS.DelayTime second.
        /// The access time is updated to maintain the priority queue for swapping.
        /// </summary>
        /// <param name="command">Represents the received command (read / write).</param>
        /// <param name="page">The page for which the request was made.</param>
        private async static Task HandleReadWriteCommand(Command command, Page page)
        {
            if (command.AccessType == PageAccessType.Write)
            {
                if(page.IsDirty)
                {
                    await OS.SavePageChangesToDisk(page);
                }

                page.IsDirty = true;
                await OS.SimulateHandling();
            }

            page.LastTimeAccessed = DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            Counter.IncrementRamAccesses(); //1 RAM access
        } 
    }
}
