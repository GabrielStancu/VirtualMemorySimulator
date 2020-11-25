using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace Machine
{
    /// <summary>
    /// A static class representing the page table of the RAM. 
    /// Our design presents a singular page table for the whole RAM.
    /// </summary>
    internal static class PageTable
    {
        internal static List<Page>? Pages { get; set; }

        internal static void LoadPages(int pageTableSize)
        {
            if(Pages == null)
            {
                Pages = new List<Page>(pageTableSize);

                for (int index = 0; index < pageTableSize; index++)
                {
                    Pages[index] = new Page(index);
                }
            }
        }

        internal static Page? GetPageByIndex(int pageIndex)
            => (Pages == null) ? null : Pages.Find(p => p.PageIndex == pageIndex);

        

        
    }
}
