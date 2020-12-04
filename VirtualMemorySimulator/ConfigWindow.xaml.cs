using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VirtualMemorySimulator
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private int _processCount;
        private int _commandsCount;
        private int _ramFrames;
        private int _pagesPerProc;
        private int _osDelay;
        private int _betweenOpsDelay;

        private readonly int _maxProcesses;

        public int ProcessCount { get => _processCount; internal set => _processCount = value; }
        public int CommandsCount { get => _commandsCount; internal set => _commandsCount = value; }
        public int RamFrames { get => _ramFrames; internal set => _ramFrames = value; }
        public int PagesPerProc { get => _pagesPerProc; internal set => _pagesPerProc = value; }
        public int OsDelay { get => _osDelay; internal set => _osDelay = value; }
        public int BetweenOpsDelay { get => _betweenOpsDelay; internal set => _betweenOpsDelay = value; }

        public ConfigWindow(int processCount, int commandsCount, int ramFrames, int pagesPerProc, int osDelay, int betweenOpsDelay, int maxProcesses)
        {
            _processCount = processCount;
            _commandsCount = commandsCount;
            _ramFrames = ramFrames;
            _pagesPerProc = pagesPerProc;
            _osDelay = osDelay;
            _betweenOpsDelay = betweenOpsDelay;
            _maxProcesses = maxProcesses;

            InitializeComponent();
        }

        private void OnProcessTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(processesCountTextBlock, _processCount, _maxProcesses);
        }

        private void OnCommandsTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(commandsCountTextBlock, _commandsCount);
        }

        private void OnRamFramesTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(ramFramesCountTextBlock, _ramFrames);
        }

        private void OnPagesPerProcTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(maxPagesPerProcessTextBlock, _pagesPerProc);
        }

        private void OnOsDelayTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(delayTimeTextBlock, _osDelay);
        }

        private void OnBetweenOpsTbLostFocus(object sender, RoutedEventArgs e)
        {
            ParseTextBoxContent(betweenOpsDelayTextBlock, _betweenOpsDelay);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ParseTextBoxContent(processesCountTextBlock, _processCount, _maxProcesses);
            ParseTextBoxContent(commandsCountTextBlock, _commandsCount);
            ParseTextBoxContent(ramFramesCountTextBlock, _ramFrames);
            ParseTextBoxContent(maxPagesPerProcessTextBlock, _pagesPerProc);
            ParseTextBoxContent(delayTimeTextBlock, _osDelay);
            ParseTextBoxContent(betweenOpsDelayTextBlock, _betweenOpsDelay);

            _processCount = Int32.Parse(processesCountTextBlock.Text);
            _commandsCount = Int32.Parse(commandsCountTextBlock.Text);
            _ramFrames = Int32.Parse(ramFramesCountTextBlock.Text);
            _pagesPerProc = Int32.Parse(maxPagesPerProcessTextBlock.Text);
            _osDelay = Int32.Parse(delayTimeTextBlock.Text);
            _betweenOpsDelay = Int32.Parse(betweenOpsDelayTextBlock.Text);
        }

        private void ParseTextBoxContent(TextBox textBox, int value, int maxValue = -1)
        {
            if (!Int32.TryParse(textBox.Text, out int tempValue) || tempValue <= 0 || (maxValue > 0 && tempValue > maxValue))
            {
                textBox.Text = value.ToString();
            }
        }   
    }
}
