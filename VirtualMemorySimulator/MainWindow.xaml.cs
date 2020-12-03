using Machine;
using Machine.Components;
using Machine.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private ObservableCollection<Machine.Page> ProcessPageTable;
        private int _freeRamFrames;
        private int _totalRamFrames = 8;
        private bool _extendedView = false;

        private CommandsInfo _cmdInfo;
        private RamInfo _ramInfo;

        public MainWindow()
        {
            InitializeComponent();     
            this.RamGauge.DataContext = new GaugeViewModel();
            SetColumnNames();
            OS.RamFramesChanged += OnRamFramesChanged;
            FreeRamFramesLabel.Content = $"{_totalRamFrames} out of {_totalRamFrames}";
        }

        private async void Simulate()
        {
            SimulationStartButton.IsEnabled = false;
            CommandsTabButton.IsEnabled = true;

            Counter.PropertyChanged += OsCounterPropertyChanged;
            _simulation = OS.Run(ramFrames:_totalRamFrames);       
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
            if (_extendedView)
            {
                _cmdInfo.Visibility = Visibility.Hidden;
                _ramInfo.Visibility = Visibility.Hidden;
                _extendedView = false;
            }
            else
            {
                _cmdInfo.Visibility = Visibility.Visible;
                _ramInfo.Visibility = Visibility.Visible;
                _extendedView = true;
            }
        }

        private void GetProcessPageTableInfo(string processName)
        {
            int pid = Int32.Parse(processName[1].ToString());
            PageTable pt = OS.GetRunningProcesses()[pid].PageTable;
            ProcessPageTable = new ObservableCollection<Machine.Page>(pt.GetPageTableInfo());
            dgProcessPageTable.Columns.Clear();
            dgProcessPageTable.ItemsSource = ProcessPageTable;
            SetColumnNames();
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
            int offset = 12;
            int cmdInfoWidth = 370;

            Simulate();
            GetProcessPageTableInfo("p0");
            GetProcessesDetails();

            _cmdInfo = new CommandsInfo();
            _cmdInfo.Show();
            _cmdInfo.Left = this.Left - cmdInfoWidth - offset;
            _cmdInfo.Top = this.Top;

            _ramInfo = new RamInfo();
            _ramInfo.Show();
            _ramInfo.Left = this.Left + this.Width - offset;
            _ramInfo.Top = this.Top;

            _extendedView = true;
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

        private void SetColumnNames()
        {
            dgProcessPageTable.Columns[0].Header = "Is Valid";
            dgProcessPageTable.Columns[1].Header = "Page Index";
            dgProcessPageTable.Columns[2].Header = "Is Dirty";
            dgProcessPageTable.Columns[3].Header = "Requested";
            dgProcessPageTable.Columns[4].Header = "Last Access";
        }

        private void OsCounterPropertyChanged(object sender, EventArgs e)
        {
            RamAccessesLabel.Content = Counter.RamAccesses;
            DiskAccessesLabel.Content = Counter.DiskAccesses;
            PageFaultsLabel.Content = Counter.PageFaults;
            PageSwapsLabel.Content = Counter.PageSwaps;
        }

        private void OnRamFramesChanged(object sender, EventArgs e)
        {
            _freeRamFrames = OS.FreeRamFrames;
            FreeRamFramesLabel.Content = $"{_freeRamFrames} out of {_totalRamFrames}";

            //here we will perform the gauge update once it works
        }
    }
}
