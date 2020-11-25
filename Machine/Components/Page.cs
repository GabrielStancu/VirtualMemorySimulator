using System;

namespace Machine
{
    /// <summary>
    /// A class representing the entry (one page) from the page table.
    /// </summary>
    internal class Page
    {
        /// <summary>
        /// Initializez the page at the beginning of the simulation. 
        /// Called by the PageTable once, when the OS requires the initialization of the page table.
        /// </summary>
        /// <param name="index">The index of the page in the page table.</param>
        internal Page(int index)
        {
            IsValid = false;
            PageIndex = index;
            IsDirty = false;
            Requested = 0;
            LastTimeAccessed = DateTime.Now;
        }
        /// <summary>
        /// Indicates whether this page is loaded in the RAM or not
        /// </summary>
        internal bool IsValid { get; set; }
        /// <summary>
        /// The index pf the page in the page table. 
        /// (~The address on 20 bits on the classic model.)
        /// </summary>
        internal int PageIndex { get; set; }
        /// <summary>
        /// Indicates whether the page was modified since it was loaded in the RAM.
        /// </summary>
        internal bool IsDirty { get; set; }
        /// <summary>
        /// Integer value, 0 if no process requires this page or it is loaded in the RAM.
        /// If not loaded and required by a process, this field gets that process' id. 
        /// </summary>
        internal int Requested { get; set; }
        /// <summary>
        /// The process to which this entry in the page table belongs.
        /// </summary>
        internal int ProcessId { get; set; }
        /// <summary>
        /// Stores the last access time of this page. 
        /// If the page table is full, the page whose access time is the lowest is swapped.
        /// </summary>
        internal DateTime LastTimeAccessed { get; set; }
    }
}
