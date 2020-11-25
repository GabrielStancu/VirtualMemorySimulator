namespace Machine
{
    public static class Counter
    {
        private static int _ramAccesses = 0;
        private static int _diskAccesses = 0;
        private static int _pageFaults = 0;

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
