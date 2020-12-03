using Machine;
using Machine.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace VirtualMemorySimulator
{
    /// <summary>
    /// Interaction logic for RamInfo.xaml
    /// </summary>
    public partial class RamInfo : Window
    {
        private ObservableCollection<RamFrame> _ramFrames;
        public RamInfo()
        {
            InitializeComponent();
            FillRamFrames();
            OS.RamFramesChanged += LoadNewFrame;
        }

        private void FillRamFrames()
        {
            _ramFrames = new ObservableCollection<RamFrame>(OS.GetRamFrames());
            dgRam.ItemsSource = _ramFrames;
        }

        private void LoadNewFrame(object sender, EventArgs e)
        {
            _ramFrames.Add(OS.RamFramesTable[^1]);
        }
    }
}
