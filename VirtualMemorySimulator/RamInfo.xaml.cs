using Machine;
using Machine.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace VirtualMemorySimulator
{
    /// <summary>
    /// Interaction logic for RamInfo.xaml
    /// </summary>
    public partial class RamInfo : Window
    {
        public RamInfo()
        {
            InitializeComponent();
            dgRam.ItemsSource = OS.GetRamFrames();
        }
    }
}
