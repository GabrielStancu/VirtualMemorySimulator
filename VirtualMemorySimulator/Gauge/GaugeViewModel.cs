using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace VirtualMemorySimulator.Gauge
{
    public class GaugeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName]string info = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        int _maxValue;
        public GaugeViewModel()
        {
            Angle = -90;
            Value = 0;
            _maxValue = 100;
        }

        int _angle;
        public int Angle
        {
            get { return _angle;}
            set
            {
                if(_angle != value)
                {
                    _angle = value;
                    NotifyPropertyChanged("Angle");
                }
                
            }
        }

        int _value;
        public int Value
        {
            get { return _value;}
            set
            {
                if (value >= 0 && value <= _maxValue && _value != value)
                {              
                    _value = value;
                    Angle = (int)(1.0 * _value / 100 * 180) - 90;
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }
}
