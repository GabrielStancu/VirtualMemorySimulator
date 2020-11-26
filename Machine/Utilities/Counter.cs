namespace Machine
{
    /// <summary>
    /// Class used to count and return the number of accesses of the memory.
    /// </summary>
    public static class Counter
    {
        private static int _ramAccesses = 0;
        private static int _diskAccesses = 0;
        private static int _pageFaults = 0;

        /// <summary>
        /// Returns for display purposes the counted values of the system.
        /// </summary>
        /// <returns>A tuple containing the ram accesses count, the disk accesses count and the page faults.</returns>
        public static (int ramAccesses, int diskAccesses, int pageFaults) GetStats()
            => (_ramAccesses, _diskAccesses, _pageFaults);

        internal static void IncrementRamAccesses()
            => _ramAccesses++;

        internal static void IncrementDiskAccesses()
            => _diskAccesses++;

        internal static void IncrementPageFaults()
            => _ramAccesses++;
    }
}
