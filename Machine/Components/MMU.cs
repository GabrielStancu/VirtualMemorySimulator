using Machine.Components;
using Machine.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Machine.Components
{
    /// <summary>
    /// Class used to simulate the memory management unit (MMU).
    /// Performs accesses on the page table, notifies the OS when elevated operations are needed (load page from disk, save page to disk etc.).
    /// </summary>
    internal static class MMU
    {
        /// <summary>
        /// Asynchronous method that runs each command on a separate Task / thread.
        /// </summary>
        /// <param name="commands">The commands to be run during the simulation.</param>
        /// <param name="processes">The running processes of the simulation.</param>
        /// <param name="betweenOpsDelayTime">The delay time between operations. Introduced for better tracking during the simulation,
        /// but also for simulating the real-life time needed for OS to switch between operations.</param>
        internal async static Task Run(IReadOnlyList<Command> commands, IReadOnlyList<Process> processes, int betweenOpsDelayTime)
        {
            for (int index = 0; index < commands.Count; index++)
            {
                int pid = commands[index].ProcessId;
                Page page = processes[pid].PageTable.GetPageByIndex(commands[index].PageIndex);
                await AccessPage(commands[index], page);
                commands[index].Completed = true;
                OS.OnCommandFinished(commands[index]);
                await Task.Delay(betweenOpsDelayTime);
            }
        }

        /// <summary>
        /// Method used by MMU to simulate the access of a page.
        /// First it checks whether the page exists in RAM and it belongs to the calling process, 
        /// then it checks whether the page is loaded. If not, the page is loaded then the read / write command is executed. 
        /// </summary>
        /// <param name="command">The command to be processed by the MMU.</param>
        /// <param name="page">The page that's required to be accessed for reading or writing.</param>
        private async static Task AccessPage(Command command, Page page)
        {
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
        /// </summary>
        /// <param name="command">Represents the received command (read / write).</param>
        /// <param name="page">The page for which the request was made.</param>
        private async static Task HandlePageRequested(Command command, Page page)
        {
            page.Requested = command.ProcessId;
            page.LastTimeAccessed = CurrentTimeGetter.GetCrtTime();
            await OS.LoadPage(page);
            page.Requested = -1;
            page.IsValid = true;
        }

        /// <summary>
        /// Method that handles the actual read / write command.
        /// If the command is write, it simulates the handling by asking the OS to save changes to disk (if page is dirty)
        /// and resets the dirty bit.
        /// </summary>
        /// <param name="command">Represents the received command (read / write).</param>
        /// <param name="page">The page for which the request was made.</param>
        private async static Task HandleReadWriteCommand(Command command, Page page)
        {
            page.LastTimeAccessed = CurrentTimeGetter.GetCrtTime();

            if (command.AccessType == PageAccessType.Write)
            {
                if(page.IsDirty)
                {
                    await OS.SavePageChangesToDisk(page);
                }

                page.IsDirty = true;
            }   
        } 
    }
}
