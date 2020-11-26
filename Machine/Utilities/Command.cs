using System;
using System.Collections.Generic;
using System.Text;

namespace Machine.Utilities
{
    /// <summary>
    /// Class representing a command to be processed by the MMU.
    /// </summary>
    internal class Command
    {
        /// <summary>
        /// Initizlizes the properties of the class.
        /// </summary>
        /// <param name="pageIndex">The index of the page to be accessed in the page table.</param>
        /// <param name="processId">The process requesting the page access.</param>
        /// <param name="accessType">The operation type: read / write.</param>
        internal Command(int pageIndex, int processId, PageAccessType accessType)
        {
            PageIndex = pageIndex;
            ProcessId = processId;
            AccessType = accessType;
        }

        /// <summary>
        /// The index of the page to be accessed in the page table.
        /// </summary>
        internal int PageIndex { get; }

        /// <summary>
        /// The process requesting the page access.
        /// </summary>
        internal int ProcessId { get; }

        /// <summary>
        /// The operation type: read / write.
        /// </summary>
        internal PageAccessType AccessType { get; }
    }
}
