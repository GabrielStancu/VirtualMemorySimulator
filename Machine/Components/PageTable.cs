using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
    /// <summary>
    /// Class representing the page table of a process. 
    /// Our design presents a page table / running process.
    /// </summary>
    public class PageTable
    {
        public List<Page> Pages { get; private set; }

        internal PageTable(int pageTableSize)
        {
            Pages = new List<Page>(pageTableSize);
            LoadPages(pageTableSize);
        }

        private void LoadPages(int pageTableSize)
        {
            for(int index = 0; index < pageTableSize; index++)
            {
                Pages.Add(new Page(index));
            }
        }

        internal Page GetPageByIndex(int pageIndex)
            => Pages.Find(p => p.PageIndex == pageIndex);
    }
}
