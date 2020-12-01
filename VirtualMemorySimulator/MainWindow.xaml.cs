using Machine;
using Machine.Components;
using Machine.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VirtualMemorySimulator.Gauge;

namespace VirtualMemorySimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Task _simulation;
        private static int _processCount = 8;
        public MainWindow()
        {
            InitializeComponent();    
            this.RamGauge.DataContext = new GaugeViewModel();
        }

        private async void Simulate()
        {
            SimulationStartButton.IsEnabled = false;
            CommandsTabButton.IsEnabled = true;

            _simulation = OS.Run();
            await _simulation;

            SimulationStartButton.IsEnabled = true;
            CommandsTabButton.IsEnabled = false;
        }

        private void ProcessBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string processName = ((Border)sender).Name;
            GetProcessPageTableInfo(processName);
        }

        private void OnCommandsTabClicked(object sender, RoutedEventArgs e)
        {
            CommandsInfo cmdInfo = new CommandsInfo();
            cmdInfo.Show();
        }

        private void GetProcessPageTableInfo(string processName)
        {
            int pid = Int32.Parse(processName[1].ToString()) - 1;
            PageTable pt = OS.GetRunningProcesses()[pid].PageTable;
            dgProcessPageTable.ItemsSource = pt.GetPageTableInfo();
            SwitchProcessBorderColor(processName);
        }

        private void SwitchProcessBorderColor(string processName)
        {
            foreach (var child in HeaderGrid.Children)
            {
                if (child.GetType() == typeof(Border))
                {
                    Border border = (Border)child;
                    if (!border.Name.Equals(processName))
                    {
                        border.Background = Brushes.CadetBlue;
                    }
                    else
                    {
                        border.Background = new SolidColorBrush(Color.FromRgb(67, 97, 238));
                    }
                }
            }
        }

        private void OnStartSimulationClicked(object sender, RoutedEventArgs e)
        {
            Simulate();
            GetProcessPageTableInfo("p1");
            GetProcessesDetails();
        }

        private void GetProcessesDetails()
        {
            List<Command> commands = new List<Command>(OS.GetCommands());
            List<Label> commandsLabels = new List<Label> { p1CommandsLabel, p2CommandsLabel, p3CommandsLabel, p4CommandsLabel,
                                                           p5CommandsLabel, p6CommandsLabel, p7CommandsLabel, p8CommandsLabel};

            for(int pid = 0; pid < _processCount; pid++)
            {
                commandsLabels[pid].Content = commands.FindAll(c => c.ProcessId == pid).Count;
            }

            List<Process> processes = new List<Process>(OS.GetRunningProcesses());
            List<Label> pageTableSizeLabels = new List<Label> { p1PageTableSizeLabel, p2PageTableSizeLabel, p3PageTableSizeLabel, p4PageTableSizeLabel,
                                                                p5PageTableSizeLabel, p6PageTableSizeLabel, p7PageTableSizeLabel, p8PageTableSizeLabel};

            for(int pid = 0; pid < _processCount; pid++)
            {
                pageTableSizeLabels[pid].Content = processes[pid].PageTableSize;
            }
            
        }
    }
}
