﻿using Machine.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Machine
{
#nullable enable
    /// <summary>
    /// Class used to simulate the memory management unit (MMU).
    /// Performs accesses on the page table and counts the accesses of memory (RAM and Disk).
    /// </summary>
    internal class MMU
    {
        /// <summary>
        /// Asynchronous method that assigns each request to a Task object.
        /// Each Task is run. If the returned value is false => ~Segmentation Fault, notify the OS to kill the process.
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        internal async static Task Run(IReadOnlyList<Command> commands)
        {
            Task<bool>[] cmds = new Task<bool>[commands.Count];

            for (int index = 0; index < commands.Count; index++)
            {
                cmds[index] = AccessPage(commands[index]);
                cmds[index].Start();

                bool succeeded = await cmds[index];
                if (succeeded == false)
                {
                    OS.KillProcess(commands[index].ProcessId);
                }
            }
        }

        /// <summary>
        /// Method used by MMU to simulate the access of a page.
        /// First it checks whether the page exists in RAM and it belongs to the calling process, 
        /// then it checks whether the page is loaded. If not, the page is loaded then the read / write command is executed. 
        /// </summary>
        /// <param name="command">The command to be processed by the MMU.</param>
        /// <returns>Returns true if the read / write was successfull, false if the page index is not valid or the page does not belong to the process.</returns>
        private async static Task<bool> AccessPage(Command command)
        {
            Page? page = PageTable.GetPageByIndex(command.PageIndex);
            Counter.IncrementRamAccesses(); //1 RAM access 

            if (page != null && page.ProcessId == command.ProcessId)
            {
                if (!page.IsValid)
                {
                    await HandlePageRequested(command, page);
                }

                await HandleReadWriteCommand(command, page);

                return true;
            }

            return false;
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
            Task<bool> loadPageTask = OS.LoadPage(page);
            loadPageTask.Start();
            await loadPageTask;

            page.Requested = 0;
            page.LastTimeAccessed = DateTime.Now;
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
            page.LastTimeAccessed = DateTime.Now;
            Counter.IncrementRamAccesses(); //1 RAM access
        } 
    }
}
