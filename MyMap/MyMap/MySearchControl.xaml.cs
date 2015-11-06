using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;

namespace MyMap
{
    public partial class MySearchControl : UserControl
    {
        public MySearchControl()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return txtSearch.Text; }
            set { txtSearch.Text = value; }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            btnClose.Width = btnClose.Height = this.Height;
            txtSearch.Height = this.Height;
            txtSearch.Width = this.Width - btnClose.Width + 20 < 0 ? 0 : this.Width - btnClose.Width + 20;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            SlideSearchOff.Begin();
        }

        public void TextboxFocus()
        {
            txtSearch.Focus();
            txtSearch.SelectAll();
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.Opacity = 1;
            btnClose.Opacity = 1;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.Opacity = 0.2;
            btnClose.Opacity = 0.2;
        }
    }
}
