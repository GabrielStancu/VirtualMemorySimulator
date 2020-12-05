﻿namespace Machine.Components
{
    /// <summary>
    /// Class representing the process (a program's run) in our simulation.
    /// </summary>
    public class Process
    {
        /// <summary>
        /// The process identifier.
        /// </summary>
        public int Pid { get; private set; }
        /// <summary>
        /// The size of the page table assigned to this process.
        /// </summary>
        public int PageTableSize { get; private set; }
        /// <summary>
        /// The actual page table assigned to this process.
        /// </summary>
        public PageTable PageTable { get; private set; }
        /// <summary>
        /// Initializes the process class.
        /// </summary>
        /// <param name="pid">The pid to be assigned to the process.</param>
        /// <param name="pageTableSize">The page table size assigned to the process.</param>
        internal Process(int pid, PageTable pageTable)
        {
            Pid = pid;
            PageTableSize = pageTable.Pages.Count;
            PageTable = pageTable;
        }
    }
}
