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
using Machine;
using Machine.Components;
using Machine.Utilities;

namespace VirtualMemorySimulator
{
    /// <summary>
    /// Interaction logic for CommandsInfo.xaml
    /// </summary>
    public partial class CommandsInfo : Window
    {
        public CommandsInfo()
        {
            InitializeComponent();
            FillCommands();
        }

        private void FillCommands()
        {
            IReadOnlyList<Command> commands = OS.GetCommands();
            dgCmds.ItemsSource = commands;
        }
    }
}
