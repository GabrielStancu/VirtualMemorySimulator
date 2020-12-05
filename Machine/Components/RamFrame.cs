using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Machine.Components
{
    /// <summary>
    /// Class representing a Ram frame. If not loaded, ProcessId and PageTableIndex are -1.
    /// </summary>
    public class RamFrame : INotifyPropertyChanged
    {
        /// <summary>
        /// The index of the frame in the frame table of the Ram.
        /// </summary>
        private int _frameIndex;
        public int FrameIndex
        {
            get { return _frameIndex; }
            internal set 
            { 
                if(_frameIndex != value)
                {
                    _frameIndex = value;
                    OnPropertyChanged("FrameIndex");
                }
                
            }
        }

        /// <summary>
        /// The process id of the frame's owner process.
        /// </summary>
        private int _processId;
        public int ProcessId
        {
            get { return _processId; }
            internal set 
            {
                if(_processId != value)
                {
                    _processId = value;
                    OnPropertyChanged("ProcessId");
                }
            }
        }

        /// <summary>
        /// The index in the page table of the process of the page mapped to this frame.
        /// </summary>
        private int _ptIndex;
        public int PtIndex
        {
            get { return _ptIndex; }
            internal set 
            {
                if (_ptIndex != value)
                {
                    _ptIndex = value;
                    OnPropertyChanged("PtIndex");
                }
            }
        }

        /// <summary>
        /// The last access time of this frame. Used for frame swapping.
        /// </summary>
        private string _lastAccess;
        public string LastAccess
        {
            get { return _lastAccess; }
            internal set 
            {
                if (_lastAccess == null || !_lastAccess.Equals(value))
                {
                    _lastAccess = value;
                    OnPropertyChanged("LastAccess");
                }
            }
        }

        /// <summary>
        /// Event fired each time one of the frame's property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Used for firing the event with the name of the property that changed.
        /// </summary>
        /// <param name="caller"></param>
        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
