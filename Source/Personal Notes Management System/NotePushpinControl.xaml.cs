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
using System.Windows.Media;

namespace Personal_Notes_Management_System
{
    public partial class NotePushpinControl : UserControl
    {
        public event EventHandler PushpinTap;
        public NotePushpinControl(object datacontext)
        {
            InitializeComponent();
            this.DataContext = datacontext;
        }

        public NotePushpinControl(object datacontext, ImageSource icon, string title)
        {
            InitializeComponent();
            Icon = icon;
            Title = title;
            this.DataContext = datacontext;
        }

        public ImageSource Icon
        {
            get { return pIcon.Source; }
            set { pIcon.Source = value; }
        }

        public string Title
        {
            get { return pName.Text; }
            set { pName.Text = value; }
        }

        private void UserControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PushpinTap != null)
                PushpinTap(sender, null);
        }
    }
}
