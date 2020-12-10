using Machine.Utilities;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Machine.Components
{
    /// <summary>
    /// A class representing the entry (one page) from the page table.
    /// </summary>
    public class Page : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializez the page at the beginning of the simulation. 
        /// Called by the PageTable once, when the OS requires the initialization of the page table.
        /// </summary>
        /// <param name="index">The index of the page in the page table.</param>
        internal Page(int index)
        {
            IsValid = false;
            PageIndex = index;
            IsDirty = false;
            Requested = -1;
            LastTimeAccessed = CurrentTimeGetter.GetCrtTime();   
        }
        /// <summary>
        /// Indicates whether this page is loaded in the RAM or not
        /// </summary>
        private bool valid;
        public bool IsValid 
        {
            get { return valid; }
            internal set
            {
                if(valid != value)
                {
                    valid = value;
                    OnPropertyChanged("IsValid");
                }
            }
        }
        /// <summary>
        /// The index pf the page in the page table. 
        /// (~The address on 20 bits on the classic model.)
        /// </summary>
        private int pageIndex;

        public int PageIndex
        {
            get { return pageIndex; }
            internal set 
            {
                if(pageIndex != value)
                {
                    pageIndex = value;
                    OnPropertyChanged("PageIndex");
                }
            }
        }

        /// <summary>
        /// Indicates whether the page was modified since it was loaded in the RAM.
        /// </summary>
        private bool isDirty;
        public bool IsDirty 
        {
            get { return isDirty; }
            internal set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    OnPropertyChanged("IsDirty");
                }
            }
        }
        /// <summary>
        /// Integer value, 0 if no process requires this page or it is loaded in the RAM.
        /// If not loaded and required by a process, this field gets that process' id. 
        /// </summary>
        private int requested;
        public int Requested 
        {
            get { return requested; }
            internal set 
            {
                if (requested != value)
                {
                    requested = value;
                    OnPropertyChanged("Requested");
                }
            }
        }
        /// <summary>
        /// Stores the last access time of this page. 
        /// If the page table is full, the page whose access time is the lowest is swapped.
        /// </summary>
        private string lastTimeAccessed;
        public string LastTimeAccessed 
        {
            get { return lastTimeAccessed; }
            internal set
            {
                if (lastTimeAccessed != value)
                {
                    lastTimeAccessed = value;
                    OnPropertyChanged("LastTimeAccessed");
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
        private void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
