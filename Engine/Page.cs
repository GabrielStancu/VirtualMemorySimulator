using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    internal class Page
    {
        private const int PAGE_SIZE = 4; //for the moment 
        private const string WritePageErrorMessage = "Page address not accessible!";

        internal int PhysicalAddressStart { get; }
        internal int PhysicalAddressEnd { get; }
        internal List<MemoryByte> PageContent { get; private set; }

        internal Page(int physicalAddressStart, int physicalAddressEnd, MemoryByte[] memoryBytes)
            => (PhysicalAddressStart, PhysicalAddressEnd, PageContent) =
            (physicalAddressStart, physicalAddressEnd, new List<MemoryByte>(memoryBytes));

        internal Page(int physicalAddressStart, int physicalAddressEnd) 
            : this(physicalAddressStart, physicalAddressEnd, new MemoryByte[PAGE_SIZE])
        {
        }

        internal (int Start, int End) GetPageLimits() 
            => (PhysicalAddressStart, PhysicalAddressEnd);

        internal void OverwritePageContent(int offset, MemoryByte[] memoryBytes)
        {
            if (offset + memoryBytes.Length >= PAGE_SIZE)
            {
                throw new InvalidAddressException(WritePageErrorMessage);
            }

            for (int index = 0; index < memoryBytes.Length; index++)
            {
                PageContent[offset + index] = memoryBytes[index];
            }
        }

        //de creeat un event, cand pagina este discarduita, trigger => os notified, overwrites content in secondary memory
    }
}
