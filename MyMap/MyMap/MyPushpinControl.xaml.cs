using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace MyMap
{
    public partial class MyPushpinControl : UserControl
    {
        public MyPushpinControl(System.Windows.Media.ImageSource imageSource = null, EventHandler<System.Windows.Input.GestureEventArgs> _event = null, object tag = null)
        {
            InitializeComponent();
            if (imageSource != null)
                image.Source = imageSource;
            if (_event != null)
                this.Tap += _event;
            this.Tag = tag;
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
