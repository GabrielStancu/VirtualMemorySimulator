using Machine.Utilities;
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
        /// Method used by MMU to simulate the access of a page.
        /// </summary>
        /// <param name="command">The command to be processed by the MMU.</param>
        /// <returns>Returns true if the read / write was successfull, false if the page index is not valid or the page does not belong to the process.</returns>
        private async static Task<bool> AccessPage(Command command)
        {
            //todo: send and receive notifications to/from the OS
            Page? page = PageTable.GetPageByIndex(command.PageIndex);
            Counter.IncrementRamAccesses(); //1 RAM access 

            if (page != null && page.ProcessId == command.ProcessId)
            {
                if (command.AccessType == PageAccessType.Write)
                {
                    page.IsDirty = true;
                    await Task.Delay(1000); //simulate the writing by sleeping 1 second
                }
                
                Counter.IncrementRamAccesses(); //1 RAM access
                return true;
            }
            else if (page != null && page.IsValid)
            {
                //todo: write pid in requested, notify OS to load the page (=> sleep(1s))
                //when done, OS notifies MMU
            }

            return false;
        }

        /// <summary>
        /// Asynchronous method that assigns each request to a Task object.
        /// Each Task is run. If the returned value is false => ~Segmentation Fault, notify the OS to kill the process.
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        internal async static Task Run(List<Command> commands)
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
    }
}
