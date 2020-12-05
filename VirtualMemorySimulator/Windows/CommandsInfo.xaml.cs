using System;
using System.Windows;
using Machine;
using Machine.Utilities;

namespace VirtualMemorySimulator.Windows
{
    /// <summary>
    /// The window where the commands details and status can be checked in a DataGrid control.
    /// </summary>
    public partial class CommandsInfo : Window
    {
        /// <summary>
        /// Sets the source of the DataGrid with the commands taken from OS
        /// and maps the handler for the Event fired by OS each time a command is finished.
        /// </summary>
        public CommandsInfo()
        {
            InitializeComponent();
            dgCmds.ItemsSource = OS.GetCommands();
            OS.CommandFinished += OnCommandFinished;
        }

        /// <summary>
        /// Each time the OS fires the CommandFinished event, it is handled here by ticking the Completed value of the command in the DataGrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCommandFinished(object sender, EventArgs e)
        {
            Command lastCommand = (Command)sender;
            dgCmds.ScrollIntoView(lastCommand);
        }
    }
}
