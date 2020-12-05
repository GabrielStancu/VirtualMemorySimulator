using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VirtualMemorySimulator.Gauge
{
    /// <summary>
    /// The Model used as DataContext for the Gauge (visual control from UI).
    /// </summary>
    public class GaugeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event fired each time one of the properties changed its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method called each time a property has changed. Fires the event.
        /// </summary>
        /// <param name="info">The name of the property that has changed.</param>
        private void NotifyPropertyChanged([CallerMemberName]string info = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        /// <summary>
        /// The maximum value that can be displayed on the gauge.
        /// </summary>
        private readonly int _maxValue;

        /// <summary>
        /// The constructor of the class. Sets the initial values for the instance.
        /// </summary>
        public GaugeViewModel()
        {
            Angle = -90;
            Value = 0;
            _maxValue = 100;
        }

        /// <summary>
        /// The angle the "needle" of tha gauge will be rotated at.
        /// </summary>
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

        /// <summary>
        /// The value to be displayed on the gauge. Changing it reflects the change on the angle as well.
        /// </summary>
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
