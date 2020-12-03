using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Machine
{
    /// <summary>
    /// Class used to count and return the number of accesses of the memory.
    /// </summary>
    public static class Counter
    {
        internal static void ResetCounter()
        {
            RamAccesses = 0;
            DiskAccesses = 0;
            PageFaults = 0;
            PageSwaps = 0;
        }

        public static int RamAccesses { get; private set; }
        public static int DiskAccesses { get; private set; }
        public static int PageFaults { get; private set; }
        public static int PageSwaps { get; private set; }

        internal static void IncrementRamAccesses()
        {
            RamAccesses++;
            OnPropertyChanged();
        }

        internal static void IncrementDiskAccesses()
        {
            DiskAccesses++;
            OnPropertyChanged();
        }

        internal static void IncrementPageFaults()
        {
            PageFaults++;
            OnPropertyChanged();
        }

        internal static void IncrementPageSwaps()
        {
            PageSwaps++;
            OnPropertyChanged();
        }

        public static event EventHandler PropertyChanged;

        private static void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(null, new EventArgs());
        }
    }
}
