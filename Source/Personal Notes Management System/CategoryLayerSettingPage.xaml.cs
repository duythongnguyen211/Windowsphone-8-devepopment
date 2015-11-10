using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Personal_Notes_Management_System.Model;
using System.Windows.Media;
using Personal_Notes_Management_System.Converter;

namespace Personal_Notes_Management_System
{
    public partial class CategoryLayerSettingPage : PhoneApplicationPage
    {
        List<ToDoCategory> backuplist;
        public CategoryLayerSettingPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
            backuplist = App.ViewModel.CategoriesList;
        }

        private void btnSaveNote_Click(object sender, EventArgs e)
        {
            App.ViewModel.SaveChangesToDB();
            NavigationService.GoBack();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            App.ViewModel.CategoriesList = backuplist;
            App.ViewModel.SaveChangesToDB();
            NavigationService.GoBack();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            App.ViewModel.CategoriesList = backuplist;
            App.ViewModel.SaveChangesToDB();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).Background = new SolidColorBrush(ColorConverter.MyColorTemplate);
            (sender as CheckBox).BorderBrush = new SolidColorBrush(Colors.Purple);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).Background = new SolidColorBrush(Colors.White);
            (sender as CheckBox).BorderBrush = new SolidColorBrush(ColorConverter.MyColorTemplate);
        }
    }
}