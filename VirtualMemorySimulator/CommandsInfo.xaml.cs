using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
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
            dgCmds.ItemsSource = OS.GetCommands();
            OS.CommandFinished += OnCommandFinished;
        }

        private void OnCommandFinished(object sender, EventArgs e)
        {
            Command lastCommand = (Command)sender;
            dgCmds.ScrollIntoView(lastCommand);
        }
    }
}
