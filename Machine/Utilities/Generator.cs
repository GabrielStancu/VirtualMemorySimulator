using Machine.Components;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Machine.Utilities
{
    /// <summary>
    /// Class that generates the commands for the simulation based on the given parameters to the OS Run method.
    /// </summary>
    internal class Generator
    {
        /// <summary>
        /// A pseudo random number generator for the command type, process id and page table index.
        /// </summary>
        private static Random _rand = new Random();

        /// <summary>
        /// Generates a list of random commands for the simulation.
        /// </summary>
        /// <returns>The list of randomly-generated commands.</returns>
        internal IReadOnlyList<Command> GenerateCommands(int commandsCount, int processesCount, List<Process> processes)
        {
            List<Command> commands = new List<Command>(commandsCount);

            //we make sure each process executes at least one operation
            for (int cmdId = 0; cmdId < processesCount; cmdId++)
            {
                commands.Add(GenerateCommand(processes, cmdId));
            }

            //now it can go "full-random" with respect to the process id
            for(int cmdId = processesCount; cmdId < commandsCount; cmdId++)
            {
                commands.Add(GenerateCommand(processes));
            }

            return commands.AsReadOnly();
        }

        /// <summary>
        /// Generates the processes that will be run during our simulation.
        /// </summary>
        /// <returns>The list of processes to be run during the simulation.</returns>
        internal List<Process> GenerateProcesses(int processesCount, int maxPagesPerProcess)
        {
            List<Process> processes = new List<Process>(processesCount);

            for (int pid = 0; pid < processesCount; pid++)
            {
                PageTable pageTable = new PageTable(Generate(1, maxPagesPerProcess));
                Process process = new Process(pid, pageTable);
                processes.Add(process);
            }

            return processes;
        }

        internal List<RamFrame> GenerateRamFrames(int size)
        {
            List<RamFrame> ramFrames = new List<RamFrame>(size);
            const int UNDEFINED = -1;
            string initTime = CurrentTimeGetter.GetCrtTime();

            for (int frameIndex = 0; frameIndex < size; frameIndex++)
            {
                ramFrames.Add(new RamFrame()
                {
                    FrameIndex = frameIndex,
                    ProcessId = UNDEFINED,
                    PtIndex = UNDEFINED,
                    LastAccess = initTime
                });
            }

            return ramFrames;
        }

        /// <summary>
        /// Generates one command to be appended to the simulation's commands list.
        /// </summary>
        /// <param name="pid">Optional parameter. If given a value, the command will be assigned to given pid.
        /// Otherwise the process id is also randomly generated.</param>
        /// <returns></returns>
        private Command GenerateCommand(List<Process> processes, int pid = -1)
        {
            //generate random operation: read / write 
            PageAccessType op = GenerateOperationType();
            
            //pid = -1 => generate random pid (0..OS.ProcessCount) else use that pid 
            if (pid == -1)
            {
                pid = Generate(0, processes.Count);
            }

            //generate random page number (0..Process.PageTableSize)
            int pageIndex = Generate(0, processes[pid].PageTableSize);

            return new Command(pid, pageIndex, op);
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
