namespace Engine
{
    internal class MemoryByte
    {
        private byte[] _internalBits;

        public MemoryByte() => _internalBits = new byte[8];
        public MemoryByte(byte[] bits) => _internalBits = bits;
        
        public byte[] GetByteInfo() => _internalBits;
        internal void SetByteInfo(byte[] bits) => _internalBits = bits;
    }
}
