﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KBrowser
{
    /// <summary>
    /// TabHeader.xaml 的交互逻辑
    /// </summary>
    public partial class TabHeader : UserControl
    {
        public TabHeader()
        {
            InitializeComponent();
        }

        public TabHeader(string title)
            : this()
        {
            PageTitle.Text = title;
        }

    }
}
