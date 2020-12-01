using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Machine.Utilities
{
    /// <summary>
    /// Class representing a command to be processed by the MMU.
    /// </summary>
    public class Command : INotifyPropertyChanged
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
            completed = false;
        }

        /// <summary>
        /// The index of the page to be accessed in the page table.
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// The process requesting the page access.
        /// </summary>
        public int ProcessId { get; }

        /// <summary>
        /// The operation type: read / write.
        /// </summary>
        public PageAccessType AccessType { get; }

        private bool completed;

        public bool Completed
        {
            get { return completed; }
            set 
            {
                if (completed != value)
                {
                    completed = value;
                    OnPropertyChanged("Completed");
                }
            }
        }

        /// <summary>
        /// Event fired each time one of the properties is changed, to signal the UI and update the controls.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method called each time a property is changed. Used for firing the update enent.
        /// </summary>
        /// <param name="caller">The name of the property that changed.</param>
        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
