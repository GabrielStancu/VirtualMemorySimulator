using System.ComponentModel;

namespace VirtualMemorySimulator.Gauge
{
    public class GaugeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public GaugeViewModel()
        {
            Angle = 0;
            Value = 0;
        }

        int _angle;
        public int Angle
        {
            get
            {
                return _angle;
            }

            private set
            {
                _angle = value;
                NotifyPropertyChanged("Angle");
            }
        }

        int _value;
        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (value >= 0 && value <= 170)
                {
                    _value = (int)(1.0 * value / 170 * 100);
                    Angle = value - 85;
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }
}
