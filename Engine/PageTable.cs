using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    internal static class PageTable
    {
        private struct MainMemoryPage
        {
            Page Page;
            Process Process;
            bool IsEmpty;
            bool IsDirty;
        }

        private static List<MainMemoryPage> _pages = new List<MainMemoryPage>();


    }
}
