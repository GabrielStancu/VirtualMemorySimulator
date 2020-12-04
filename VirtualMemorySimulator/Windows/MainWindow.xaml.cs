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

namespace VirtualMemorySimulator.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Task _simulation;
        private ObservableCollection<Machine.Page> ProcessPageTable;
        private int _freeRamFrames;
        private bool _extendedView = false;
        private const int MaxProcessCount = 8;
        internal GaugeViewModel GaugeViewModel { get; set; }

        private CommandsInfo _cmdInfo;
        private RamInfo _ramInfo;
        private ConfigWindow _configWindow;

        private int _processCount = 8;
        private int _totalCommands = 48;
        private int _ramFrames = 12;
        private int _pagesPerProc = 8;
        private int _osDelay = 1000;
        private int _betweenOpsDelay = 750;

        public MainWindow()
        {
            InitializeComponent();
            GaugeViewModel = new GaugeViewModel(_ramFrames);
            RamGauge.DataContext = GaugeViewModel;
            SetColumnNames();
            OS.RamFramesChanged += OnRamFramesChanged;
            OS.OsStateChanged += OnOsStateChanged;
            FreeRamFramesLabel.Content = $"{_ramFrames} out of {_ramFrames}";
        }

        private async void Simulate()
        {
            SimulationStartButton.IsEnabled = false;
            CommandsTabButton.IsEnabled = true;
            ConfigButton.IsEnabled = false;

            Counter.PropertyChanged += OsCounterPropertyChanged;
            _simulation = OS.Run(_processCount, _totalCommands, _ramFrames, _pagesPerProc, _osDelay, _betweenOpsDelay);       
            await _simulation;

            ConfigButton.IsEnabled = true;
            SimulationStartButton.IsEnabled = true;
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

            for (int pid = 0; pid < _processCount; pid++)
            {
                commandsLabels[pid].Content = commands.FindAll(c => c.ProcessId == pid).Count;
            }

            List<Process> processes = new List<Process>(OS.GetRunningProcesses());
            List<Label> pageTableSizeLabels = new List<Label> { p1PageTableSizeLabel, p2PageTableSizeLabel, p3PageTableSizeLabel, p4PageTableSizeLabel,
                                                                p5PageTableSizeLabel, p6PageTableSizeLabel, p7PageTableSizeLabel, p8PageTableSizeLabel};

            for (int pid = 0; pid < _processCount; pid++)
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
            FreeRamFramesLabel.Content = $"{_freeRamFrames} out of {_ramFrames}";

            //here we will perform the gauge update once it works
            GaugeViewModel.Value = _freeRamFrames;
        }

        private void OnOsStateChanged(object sender, EventArgs e)
        {
            OsState state = (OsState)sender;

            switch(state)
            {
                case OsState.Busy:
                    OsStateLabel.Content = "OS: Busy";
                    OsStateBorder.Background = new SolidColorBrush(Color.FromRgb(214, 40, 40));
                    break;
                case OsState.Idle:
                    OsStateLabel.Content = "OS: Idle";
                    OsStateBorder.Background = new SolidColorBrush(Color.FromRgb(252, 163, 17));
                    break;
                default:
                    OsStateLabel.Content = "OS: Free";
                    OsStateBorder.Background = new SolidColorBrush(Color.FromRgb(6, 214, 160));
                    break;
            }
        }

        private void OnConfigButtonClicked(object sender, RoutedEventArgs e)
        {
            _configWindow = new ConfigWindow(_processCount, _totalCommands, _ramFrames, _pagesPerProc, _osDelay, _betweenOpsDelay, MaxProcessCount);
            _configWindow.Closing += _configWindow_Closing;
            _configWindow.Show();
        }

        private void _configWindow_Closing(object sender, CancelEventArgs e)
        {
            _processCount = _configWindow.ProcessCount;
            _totalCommands = _configWindow.CommandsCount;
            _ramFrames = _configWindow.RamFrames;
            _pagesPerProc = _configWindow.PagesPerProc;
            _osDelay = _configWindow.OsDelay;
            _betweenOpsDelay = _configWindow.BetweenOpsDelay;

            ConfigureProcesses();
        }

        private void ConfigureProcesses()
        {
            List<Border> processBorders = new List<Border>{ p0, p1, p2, p3, p4, p5, p6, p7 };

            for(int pid = 0; pid<_processCount; pid++)
            {
                processBorders[pid].IsEnabled = true;
                processBorders[pid].Background = Brushes.CadetBlue;
            }

            for(int pid = _processCount; pid < MaxProcessCount; pid++)
            {
                processBorders[pid].IsEnabled = false;
                processBorders[pid].Background = Brushes.Gray;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            WindowState ws = WindowState;

            if (_extendedView)
            {
                _cmdInfo.WindowState = ws;
                _ramInfo.WindowState = ws;
            }

            if (_configWindow != null)
            {
                _configWindow.WindowState = ws;
            }
        }
    }
}
