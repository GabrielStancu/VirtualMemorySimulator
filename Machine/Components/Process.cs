using Machine.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machine.Components
{
    public class Process
    {
        public int Pid { get; private set; }

        public int PageTableSize { get; private set; }

        public PageTable PageTable { get; private set; }

        public bool IsRunning { get; private set; }

        internal Process(int pid, int pageTableSize)
        {
            Pid = pid;
            PageTableSize = pageTableSize;
            PageTable = new PageTable(pageTableSize);
            IsRunning = false;
        }

        internal void SwitchProcessState()
            => IsRunning = !IsRunning;
    }
}
