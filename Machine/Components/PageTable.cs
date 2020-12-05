using System.Collections.Generic;

namespace Machine
{
    /// <summary>
    /// Class representing the page table of a process. 
    /// Our design presents a page table / running process.
    /// </summary>
    public class PageTable
    {
        /// <summary>
        /// The constructor of the class, initializez the page table
        /// </summary>
        /// <param name="pageTableSize">The number of pages the process needs for loading its entire data in RAM.</param>
        internal PageTable(int pageTableSize)
        {
            Pages = new List<Page>(pageTableSize);
            LoadPages(pageTableSize);
        }

        /// <summary>
        /// The list of pages from the PageTable.
        /// </summary>
        internal List<Page> Pages { get; private set; }

        /// <summary>
        /// Gets the page table (the internal list of pages) of this class, read-only.
        /// </summary>
        /// <returns>A read-only copy of the list of pages from this page table.</returns>
        public IReadOnlyList<Page> GetPageTableInfo()
        {
            return Pages.AsReadOnly();
        }

        /// <summary>
        /// Gets a page from the internal list of pages of this class.
        /// </summary>
        /// <param name="pageIndex">The index of the page to be retrieved.</param>
        /// <returns>The page from the requested index.</returns>
        internal Page GetPageByIndex(int pageIndex)
            => Pages.Find(p => p.PageIndex == pageIndex);

        /// <summary>
        /// Initializez the list, puts a page at each index and assigns that index to that page.
        /// </summary>
        /// <param name="pageTableSize">The size of the process' page table.</param>
        private void LoadPages(int pageTableSize)
        {
            for(int index = 0; index < pageTableSize; index++)
            {
                Pages.Add(new Page(index));
            }
        }
    }
}
