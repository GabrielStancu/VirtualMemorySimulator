using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Machine
{
    /// <summary>
    /// A class representing the entry (one page) from the page table.
    /// </summary>
    public class Page// : IComparer<Page>
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
        public bool IsValid { get; internal set; }
        /// <summary>
        /// The index pf the page in the page table. 
        /// (~The address on 20 bits on the classic model.)
        /// </summary>
        public int PageIndex { get; internal set; }
        /// <summary>
        /// Indicates whether the page was modified since it was loaded in the RAM.
        /// </summary>
        public bool IsDirty { get; internal set; }
        /// <summary>
        /// Integer value, 0 if no process requires this page or it is loaded in the RAM.
        /// If not loaded and required by a process, this field gets that process' id. 
        /// </summary>
        public int Requested { get; internal set; }
        /// <summary>
        /// Stores the last access time of this page. 
        /// If the page table is full, the page whose access time is the lowest is swapped.
        /// </summary>
        public DateTime LastTimeAccessed { get; internal set; }

        //public int Compare(Page p1, Page p2)
        //{
        //    DateTime dt1 = p1.LastTimeAccessed;
        //    DateTime dt2 = p2.LastTimeAccessed;

        //    return dt1.CompareTo(dt2);
        //}
    }
}
