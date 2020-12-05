using System;

namespace Machine
{
    /// <summary>
    /// Class used to count and return the number of accesses of the memory.
    /// </summary>
    public static class Counter
    {
        /// <summary>
        /// Resets the counted values for all kind of accesses.
        /// </summary>
        internal static void ResetCounter()
        {
            RamAccesses = 0;
            DiskAccesses = 0;
            PageFaults = 0;
            PageSwaps = 0;
        }

        /// <summary>
        /// Property that counts the number of RAM accesses.
        /// </summary>
        public static int RamAccesses { get; private set; }
        /// <summary>
        /// Property that counts the number of Disk accesses.
        /// </summary>
        public static int DiskAccesses { get; private set; }
        /// <summary>
        /// Property that counts the number of page faults.
        /// </summary>
        public static int PageFaults { get; private set; }
        /// <summary>
        /// Property that counts the number of page swaps.
        /// </summary>
        public static int PageSwaps { get; private set; }

        /// <summary>
        /// Increments the number of RAM accesses.
        /// </summary>
        internal static void IncrementRamAccesses()
        {
            RamAccesses++;
            OnPropertyChanged();
        }

        /// <summary>
        /// Increments the number of Disk accesses.
        /// </summary>
        internal static void IncrementDiskAccesses()
        {
            DiskAccesses++;
            OnPropertyChanged();
        }

        /// <summary>
        /// Increments the number of page faults.
        /// </summary>
        internal static void IncrementPageFaults()
        {
            PageFaults++;
            OnPropertyChanged();
        }

        /// <summary>
        /// Increments the number of page swaps.
        /// </summary>
        internal static void IncrementPageSwaps()
        {
            PageSwaps++;
            OnPropertyChanged();
        }

        /// <summary>
        /// Event fired each time a property has changed.
        /// </summary>
        public static event EventHandler PropertyChanged;

        /// <summary>
        /// Method called each time a property was changed, fires the event wiith the name of the property that has changed.
        /// </summary>
        private static void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(null, new EventArgs());
        }
    }
}
