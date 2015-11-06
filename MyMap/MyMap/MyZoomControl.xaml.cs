using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MyMap
{
    public partial class MyZoomControl : UserControl
    {
        public event EventHandler ZoomCtrlInClick, ZoomCtrlOutClick;
        public MyZoomControl()
        {
            InitializeComponent();
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (ZoomCtrlInClick != null)
                ZoomCtrlInClick(this, EventArgs.Empty);
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (ZoomCtrlOutClick != null)
                ZoomCtrlOutClick(this, EventArgs.Empty);
        }
    }
}
