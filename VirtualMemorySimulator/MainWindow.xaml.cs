using Machine;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VirtualMemorySimulator.Gauge;

namespace VirtualMemorySimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.RamGauge.DataContext = new GaugeViewModel();
            Simulate();
        }

        private async void Simulate()
        {
            await OS.Run();
            MessageBox.Show("Done simulating!");
        }

        private void ProcessBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string msg = ((Border)sender).Name;
            MessageBox.Show(msg);
        }

        private void OnCommandsTabClicked(object sender, RoutedEventArgs e)
        {
            CommandsInfo cmdInfo = new CommandsInfo();
            cmdInfo.Show();
        }
    }
}
