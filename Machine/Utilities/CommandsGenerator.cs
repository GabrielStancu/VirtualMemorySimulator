using System;
using System.Collections.Generic;
using System.Text;

namespace Machine.Utilities
{
    /// <summary>
    /// Class that generates the commands for the simulation based on the given parameters to the OS Run method.
    /// </summary>
    internal class CommandsGenerator
    {
        /// <summary>
        /// A pseudo random number generator for the command type, process id and page table index.
        /// </summary>
        private static Random _rand = new Random();

        /// <summary>
        /// Generates a list of random commands for the simulation.
        /// </summary>
        /// <returns>The list of randomly-generated commands.</returns>
        internal IReadOnlyList<Command> Generate()
        {
            List<Command> commands = new List<Command>(OS.CommandsCount);

            //we make sure each process executes at least one operation
            for (int cmdId = 0; cmdId < OS.ProcessCount; cmdId++)
            {
                commands.Add(GenerateCommand(cmdId));
            }

            //now it can go "full-random" with respect to the process id
            for(int cmdId = OS.ProcessCount; cmdId < OS.CommandsCount; cmdId++)
            {
                commands.Add(GenerateCommand());
            }

            return commands.AsReadOnly();
        }

        /// <summary>
        /// Generates one command to be appended to the simulation's commands list.
        /// </summary>
        /// <param name="pid">Optional parameter. If given a value, the command will be assigned to given pid.
        /// Otherwise the process id is also randomly generated.</param>
        /// <returns></returns>
        private Command GenerateCommand(int pid = -1)
        {
            //generate random operation: read / write 
            PageAccessType op = GenerateOperationType();
            //generate random page number (0..OS.PageTableSize)
            int pageIndex = Generate(0, OS.PageTableSize);
            //pid = -1 => generate random pid (0..OS.ProcessCount) else use that pid 
            if (pid == -1)
            {
                pid = Generate(0, OS.ProcessCount);
            }

            return new Command(pageIndex, pid, op);
        }

        /// <summary>
        /// Generates a random value (index of operation from possible operations enum).
        /// </summary>
        /// <returns>Generated value wrapped into the actual value from the enum.</returns>
        private PageAccessType GenerateOperationType()
        {
            int opId = Generate(0, Enum.GetNames(typeof(PageAccessType)).Length);
            return (PageAccessType)opId;
        }

        /// <summary>
        /// Generates a random number between given limits.
        /// </summary>
        /// <param name="min">The lower limit, inclusive.</param>
        /// <param name="max">The upper limit, exclusive.</param>
        /// <returns></returns>
        private int Generate(int min, int max)
        {
            return _rand.Next(min, max);
        }
    }
}
