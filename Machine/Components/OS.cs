using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
#nullable enable
    public static class OS
    {
        internal async static Task<bool> SavePageChangesToDisk(int pageIndex)
        {
            Page? page = PageTable.GetPageByIndex(pageIndex);

            if (page != null && page.IsDirty)
            {
                page.IsDirty = false;
                await Task.Delay(1000); //simulate the writing on the disk by sleeping 1 second
                Counter.IncrementDiskAccesses(); //1 Disk access
                return true;
            }

            return false;
        }

        //internal async static Task<bool> LoadPage()

        internal static void KillProcess(int processId)
        {
            //todo
        }
    }
}
