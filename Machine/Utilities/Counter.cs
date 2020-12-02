using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Machine
{
    /// <summary>
    /// Class used to count and return the number of accesses of the memory.
    /// </summary>
    public class Counter
    {
        internal Counter()
        {
            RamAccesses = 0;
            DiskAccesses = 0;
            PageFaults = 0;
        }

        public int RamAccesses { get; private set; }
        public int DiskAccesses { get; private set; }
        public int PageFaults { get; private set; }

        internal void IncrementRamAccesses()
        {
            RamAccesses++;
            OnPropertyChanged();
        }


        internal void IncrementDiskAccesses()
        {
            DiskAccesses++;
            OnPropertyChanged();
        }

        internal void IncrementPageFaults()
        {
            PageFaults++;
            OnPropertyChanged();
        }

        public event EventHandler PropertyChanged;

        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(null, new EventArgs());
        }
    }
}
