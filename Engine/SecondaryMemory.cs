using System.Collections.Generic;

namespace Engine
{
    internal static class SecondaryMemory
    {
        private const int MEMORY_SIZE = 1024; //for the moment 
        private const string WriteMemoryErrorMessage = "Inaccessible address for writing data!";
        private const string ReadMemoryErrorMessage = "Inaccessible address for reading data!";
        private static readonly List<MemoryByte> _secondaryMemoryBytes = new List<MemoryByte>(MEMORY_SIZE);

        #region ReadMemoryMethods
        internal static IEnumerable<MemoryByte> ReadWholeMemory()
            => _secondaryMemoryBytes.AsReadOnly();

        internal static MemoryByte ReadByteFromAddress(int address)
            => (address < MEMORY_SIZE) ? _secondaryMemoryBytes[address] : throw new InvalidAddressException(ReadMemoryErrorMessage);

        internal static IEnumerable<MemoryByte> ReadBytesFromAddress(int address, int length)
            => (address + length < MEMORY_SIZE) ? _secondaryMemoryBytes.GetRange(address, length).AsReadOnly() : throw new InvalidAddressException(ReadMemoryErrorMessage);
        #endregion

        #region WriteMemoryMethods
        internal static void OverwriteByteFromAddress(int address, MemoryByte memoryByte)
        {
            if (address < MEMORY_SIZE)
            {
                _secondaryMemoryBytes[address].SetByteInfo(memoryByte.GetByteInfo());
            }    
            else
            {
                throw new InvalidAddressException(WriteMemoryErrorMessage);
            }
        }

        internal static void OverwriteBytesFromAddress(int address, MemoryByte[] memoryBytes)
        {
            for (int index = 0; index < memoryBytes.Length; index++)
            {
                OverwriteByteFromAddress(address + index, memoryBytes[index]);
            }
        }
        #endregion 
    }
}
