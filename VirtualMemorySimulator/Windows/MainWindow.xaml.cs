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
    /// The main window of the simulation of the Virtual Memory concept.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The processes that are run during the simulation.
        /// </summary>
        private ObservableCollection<Machine.Page> ProcessPageTable;

        /// <summary>
        /// The number of available (not loaded) RAM frames.s
        /// </summary>
        private int _freeRamFrames;

        /// <summary>
        /// Boolean value, tells if the commands and ram info windows are visible.
        /// </summary>
        private bool _extendedView = false;

        /// <summary>
        /// The maximum number of processes the window can hold for this simulation.
        /// </summary>
        private const int MaxProcessCount = 8;

        /// <summary>
        /// The object to be bound as DataContext for the gauge component.
        /// </summary>
        internal GaugeViewModel GaugeViewModel { get; set; }


        /// <summary>
        /// A reference to the assigned commands info window.
        /// </summary>
        private CommandsInfo _cmdInfo;
        
        /// <summary>
        /// A reference to the assigned ram info window.
        /// </summary>
        private RamInfo _ramInfo;

        /// <summary>
        /// A refernce to the assigned configuration window.
        /// </summary>
        private ConfigWindow _configWindow;

        /// <summary>
        /// The default value for the processes that will be run during the simulation.
        /// </summary>
        private int _processCount = 8;

        /// <summary>
        /// The default value for the commands number during the simulation.
        /// </summary>
        private int _totalCommands = 48;

        /// <summary>
        /// The default value for the RAM frames the RAM will be divided into during the simulation.
        /// </summary>
        private int _ramFrames = 8;

        /// <summary>
        /// The default value for the maximum number of pages a process can hold in its page table during the simulation.
        /// </summary>
        private int _pagesPerProc = 8;

        /// <summary>
        /// The default value for the time needed to simulate the OS moving data between memories during the simulation.
        /// </summary>
        private int _osDelay = 1000;

        /// <summary>
        /// The default value for the time needed to simulate the OS switching between commands during the simulation.
        /// </summary>
        private int _betweenOpsDelay = 750;

        /// <summary>
        /// Initializez the window and maps the event handlers to the events fired by the Machine package.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            GaugeViewModel = new GaugeViewModel();
            RamGauge.DataContext = GaugeViewModel;
            SetColumnNames();
            FreeRamFramesLabel.Content = $"{_ramFrames} out of {_ramFrames}";

            OS.RamFramesChanged += OnRamFramesChanged;
            OS.OsStateChanged += OnOsStateChanged;
            Counter.PropertyChanged += OnOsCounterPropertyChanged;   
        }

        #region Events 
        /// <summary>
        /// Auto-turns on the ram and commands windows and aligns them on the screen.
        /// </summary>
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

        /// <summary>
        /// Event fired each time a process border is clicked. Displays that process' page table.
        /// </summary>
        private void OnProcessBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string processName = ((Border)sender).Name;
            GetProcessPageTableInfo(processName);
        }

        /// <summary>
        /// Toggles the visibility of the ram and commands info windows.
        /// </summary>
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

        /// <summary>
        /// Event fired when the configuration button is pressed and the configuration window needs to be displayed.
        /// </summary>
        private void OnConfigButtonClicked(object sender, RoutedEventArgs e)
        {
            _configWindow = new ConfigWindow(_processCount, _totalCommands, _ramFrames, _pagesPerProc, _osDelay, _betweenOpsDelay, MaxProcessCount);
            _configWindow.Closing += OnConfigWindowClosing;
            _configWindow.Show();
        }

        /// <summary>
        /// Event fired when the configuration window is closed. 
        /// Overwrittes the current simulation values with those editted in the configuration window.
        /// </summary>
        private void OnConfigWindowClosing(object sender, CancelEventArgs e)
        {
            _processCount = _configWindow.ProcessCount;
            _totalCommands = _configWindow.CommandsCount;
            _ramFrames = _configWindow.RamFrames;
            _pagesPerProc = _configWindow.PagesPerProc;
            _osDelay = _configWindow.OsDelay;
            _betweenOpsDelay = _configWindow.BetweenOpsDelay;

            ConfigureProcesses();
        }

        /// <summary>
        /// Event fired each time the state of the window has changed (Minimized, Normal, Maximized).
        /// </summary>
        private void OnWindowStateChanged(object sender, EventArgs e)
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

        /// <summary>
        /// Event fired by the OS each time one of the counted values has changed, so that the UI is updated.
        /// </summary>
        private void OnOsCounterPropertyChanged(object sender, EventArgs e)
        {
            RamAccessesLabel.Content = Counter.RamAccesses;
            DiskAccessesLabel.Content = Counter.DiskAccesses;
            PageFaultsLabel.Content = Counter.PageFaults;
            PageSwapsLabel.Content = Counter.PageSwaps;
        }

        /// <summary>
        /// Event fired by the OS each time the number of available RAM frames has changed, so that the UI is updated.
        /// </summary>
        private async void OnRamFramesChanged(object sender, EventArgs e)
        {
            _freeRamFrames = OS.FreeRamFrames;
            FreeRamFramesLabel.Content = $"{_freeRamFrames} out of {_ramFrames}";
            await CreateContinuosIncreaseOnGauge();
        }

        /// <summary>
        /// Event fired by the OS each time its state has changed, so that the UI is updated.
        /// </summary>
        private void OnOsStateChanged(object sender, EventArgs e)
        {
            OsState state = (OsState)sender;

            switch (state)
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
        #endregion

        #region UtilityMethods
        /// <summary>
        /// Starts the simulation. Disables/enables some of the buttons, resets the gauge, 
        /// calls the OS for starting the simulation on another Task. When finished, the disabled controlls are enabled back.
        /// </summary>
        private async void Simulate()
        {
            SimulationStartButton.IsEnabled = false;
            CommandsTabButton.IsEnabled = true;
            ConfigButton.IsEnabled = false;
    
            GaugeViewModel.Value = 0;
            await OS.Run(_processCount, _totalCommands, _ramFrames, _pagesPerProc, _osDelay, _betweenOpsDelay);

            ConfigButton.IsEnabled = true;
            SimulationStartButton.IsEnabled = true;
        }

        /// <summary>
        /// Gets the page table of the currently selected process and displays it in the lower-left DataGrid.
        /// </summary>
        /// <param name="processName">The name of the Border associated to the selected process.</param>
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

        /// <summary>
        /// Used for showing the currently selected process Border.
        /// </summary>
        /// <param name="processName">The name of the Border associated to the selected process.</param>
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

        /// <summary>
        /// Gets the general details about each process: the page table size, the total number of commands it will execute.
        /// </summary>
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

        /// <summary>
        /// Overwrites the name of the columns of the Datagrid with more understandable values.
        /// </summary>
        private void SetColumnNames()
        {
            dgProcessPageTable.Columns[0].Header = "Is Valid";
            dgProcessPageTable.Columns[1].Header = "Page Index";
            dgProcessPageTable.Columns[2].Header = "Is Dirty";
            dgProcessPageTable.Columns[3].Header = "Requested";
            dgProcessPageTable.Columns[4].Header = "Last Access";
        }

        /// <summary>
        /// Method used to give the gauge a continuous flow: switches by 1 step from the old value towards the new one.
        /// For a more gauge-like behavior, it goes over the actual value 2 steps, then 3 back, then one ahead (fluctuation simulation).
        /// </summary>
        private async Task CreateContinuosIncreaseOnGauge()
        {
            int initValue = GaugeViewModel.Value;
            int maxValue = 100;
            int relativeFramesNumber = (int)(1.0 / _ramFrames * maxValue);
            int mappedValue = (int)(Convert.ToDouble(_ramFrames - _freeRamFrames) / _ramFrames * maxValue);
            int forwardOffset = 2;
            int backwardOffset = 3;
            int delayTime = _betweenOpsDelay / (relativeFramesNumber + forwardOffset + backwardOffset + (backwardOffset - forwardOffset));


            for (int crtValue = initValue; crtValue <= mappedValue; crtValue++)
            {
                GaugeViewModel.Value = crtValue;
                await Task.Delay(delayTime);
            }

            initValue = GaugeViewModel.Value;
            if (initValue != maxValue)
            {
                for (int crtValue = initValue; crtValue <= initValue + forwardOffset; crtValue++)
                {
                    GaugeViewModel.Value = crtValue;
                    await Task.Delay(delayTime);
                }

                initValue = GaugeViewModel.Value;
                for (int crtValue = initValue; crtValue >= initValue - backwardOffset; crtValue--)
                {
                    GaugeViewModel.Value = crtValue;
                    await Task.Delay(delayTime);
                }

                GaugeViewModel.Value++;
                await Task.Delay(delayTime);
            }
        }

        /// <summary>
        /// Sets the color of the processes' borders according to their status, 
        /// depending on the enabled status from configuration parameters or not.
        /// </summary>
        private void ConfigureProcesses()
        {
            List<Border> processBorders = new List<Border> { p0, p1, p2, p3, p4, p5, p6, p7 };

            for (int pid = 0; pid < _processCount; pid++)
            {
                processBorders[pid].IsEnabled = true;
                processBorders[pid].Background = Brushes.CadetBlue;
            }

            for (int pid = _processCount; pid < MaxProcessCount; pid++)
            {
                processBorders[pid].IsEnabled = false;
                processBorders[pid].Background = Brushes.Gray;
            }
        }
        #endregion
    }
}
