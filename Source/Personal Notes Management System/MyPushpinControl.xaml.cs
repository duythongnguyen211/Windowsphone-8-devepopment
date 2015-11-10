using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Personal_Notes_Management_System
{
    public partial class MyPushpinControl : UserControl
    {
        public MyPushpinControl()
        {
            InitializeComponent();
        }
        public System.Windows.Media.ImageSource ImageSource
        {
            get { return image.Source; }
            set { image.Source = value; }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            image.Height = this.Height;
            image.Width = this.Width;
        }

    }
}
