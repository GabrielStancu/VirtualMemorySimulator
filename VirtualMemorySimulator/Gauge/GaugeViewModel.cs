using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        public GaugeViewModel(int maxValue)
        {
            Angle = -90;
            Value = 0;
            _maxValue = maxValue;
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
                    int mappedValue = (int)(1.0 * (_maxValue - value) / _maxValue * 100);
                    _value = mappedValue;
                    Angle = (int)(1.0 * mappedValue / 100 * 180) - 90;
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }
}
