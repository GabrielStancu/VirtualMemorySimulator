using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Machine.Components
{
    public class RamFrame : INotifyPropertyChanged
    {
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

        private int _ptIndex;
        public int PtIndex
        {
            get { return _ptIndex; }
            internal set 
            {
                if (_ptIndex != value)
                {
                    _ptIndex = value;
                    OnPropertyChanged("PageTableIndex");
                }
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
