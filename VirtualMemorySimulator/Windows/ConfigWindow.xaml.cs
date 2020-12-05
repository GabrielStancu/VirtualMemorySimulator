using System;
using System.Windows;
using System.Windows.Controls;

namespace VirtualMemorySimulator.Windows
{
    /// <summary>
    /// The window where the parameters of the simulation are configured.
    /// </summary>
    public partial class ConfigWindow : Window
    {
        /// <summary>
        /// The maximum number of process supported by the simulation.
        /// </summary>
        private readonly int _maxProcesses;

        /// <summary>
        /// The number of processes that will be run during the simulation.
        /// </summary>
        public int ProcessCount { get; internal set; }

        /// <summary>
        /// The number of commands that will be executed during the simulation.
        /// </summary>
        public int CommandsCount { get; internal set; }

        /// <summary>
        /// The number of frames the RAM will be divided into during the simulation.
        /// </summary>
        public int RamFrames { get; internal set; }

        /// <summary>
        /// The maximum number of pages a process' page table can be divided into.
        /// </summary>
        public int PagesPerProc { get; internal set; }

        /// <summary>
        /// The delay time in milliseconds used to simulate the OS transferring data between RAM and Disk.
        /// </summary>
        public int OsDelay { get; internal set; }

        /// <summary>
        /// The delay time in milliseconds used to simulate the time needed by the OS to switch between commands.
        /// </summary>
        public int BetweenOpsDelay { get; internal set; }

        /// <summary>
        /// Initializes the values of the window.
        /// </summary>
        /// <param name="processCount">The number of processes that will be run during the simulation.</param>
        /// <param name="commandsCount">The number of commands that will be executed during the simulation.</param>
        /// <param name="ramFrames">The number of frames the RAM will be divided into during the simulation.</param>
        /// <param name="pagesPerProc">The maximum number of pages a process' page table can be divided into.</param>
        /// <param name="osDelay">The delay time in milliseconds used to simulate the OS transferring data between RAM and Disk.</param>
        /// <param name="betweenOpsDelay">The delay time in milliseconds used to simulate the time needed by the OS to switch between commands.</param>
        /// <param name="maxProcesses">The maximum number of process supported by the simulation.</param>
        public ConfigWindow(int processCount, int commandsCount, int ramFrames, int pagesPerProc, int osDelay, int betweenOpsDelay, int maxProcesses)
        {
            ProcessCount = processCount;
            CommandsCount = commandsCount;
            RamFrames = ramFrames;
            PagesPerProc = pagesPerProc;
            OsDelay = osDelay;
            BetweenOpsDelay = betweenOpsDelay;
            _maxProcesses = maxProcesses;

            InitializeComponent();
        }

        /// <summary>
        /// Event fired each time the max processes number textbox loses focus.
        /// </summary>
        private void OnProcessTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(processesCountTextBlock, ProcessCount, _maxProcesses);
        }

        /// <summary>
        /// Event fired each time the commands number textbox loses focus.
        /// </summary>
        private void OnCommandsTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(commandsCountTextBlock, CommandsCount);
        }

        /// <summary>
        /// Event fired each time the RAM frames number textbox loses focus.
        /// </summary>
        private void OnRamFramesTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(ramFramesCountTextBlock, RamFrames);
        }

        /// <summary>
        /// Event fired each time the max pages per process number textbox loses focus.
        /// </summary>
        private void OnPagesPerProcTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(maxPagesPerProcessTextBlock, PagesPerProc);
        }

        /// <summary>
        /// Event fired each time the OS delay time textbox loses focus.
        /// </summary>
        private void OnOsDelayTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(delayTimeTextBlock, OsDelay);
        }

        /// <summary>
        /// Event fired each time the between operations textbox loses focus.
        /// </summary>
        private void OnBetweenOpsTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(betweenOpsDelayTextBlock, BetweenOpsDelay);
        }

        /// <summary>
        /// Event fired when the window is closed. 
        /// This event does not trigger the "lose focus" events of the textboxes, so they need to be triggerred manually.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ParseTextBoxContent(processesCountTextBlock, ProcessCount, _maxProcesses);
            ParseTextBoxContent(commandsCountTextBlock, CommandsCount);
            ParseTextBoxContent(ramFramesCountTextBlock, RamFrames);
            ParseTextBoxContent(maxPagesPerProcessTextBlock, PagesPerProc);
            ParseTextBoxContent(delayTimeTextBlock, OsDelay);
            ParseTextBoxContent(betweenOpsDelayTextBlock, BetweenOpsDelay);

            ProcessCount = Int32.Parse(processesCountTextBlock.Text);
            CommandsCount = Int32.Parse(commandsCountTextBlock.Text);
            RamFrames = Int32.Parse(ramFramesCountTextBlock.Text);
            PagesPerProc = Int32.Parse(maxPagesPerProcessTextBlock.Text);
            OsDelay = Int32.Parse(delayTimeTextBlock.Text);
            BetweenOpsDelay = Int32.Parse(betweenOpsDelayTextBlock.Text);
        }

        /// <summary>
        /// Method used to validate the content of the editting textbox.
        /// </summary>
        /// <param name="textBox">The textbox under edit.</param>
        /// <param name="value">The property of the window that will store the editted value, if it is correct (numerical and below maximum allowed value).</param>
        /// <param name="maxValue">Optional, provided for certain properties that allow only finite values (e.g. the processes number).</param>
        private void ParseTextBoxContent(TextBox textBox, int value, int maxValue = -1)
        {
            if (!Int32.TryParse(textBox.Text, out int tempValue) || tempValue <= 0 || (maxValue > 0 && tempValue > maxValue))
            {
                textBox.Text = value.ToString();
            }
        }   
    }
}
