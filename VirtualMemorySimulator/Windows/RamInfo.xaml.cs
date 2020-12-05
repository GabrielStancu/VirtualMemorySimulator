using Machine;
using System.Windows;

namespace VirtualMemorySimulator.Windows
{
    /// <summary>
    /// The window where the RAM status can be tracked.
    /// </summary>
    public partial class RamInfo : Window
    {
        /// <summary>
        /// Initializes the window and gets the list of RAM frames from the OS.
        /// </summary>
        public RamInfo()
        {
            InitializeComponent();
            dgRam.ItemsSource = OS.GetRamFrames();
        }
    }
}
