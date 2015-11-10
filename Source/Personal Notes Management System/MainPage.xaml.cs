using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Personal_Notes_Management_System.Resources;
using System.Windows.Data;
using Personal_Notes_Management_System.Model;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Markup;
using System.IO;
using System.Windows.Media;
using Personal_Notes_Management_System.Converter;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Scheduler;

namespace Personal_Notes_Management_System
{
    public partial class MainPage : PhoneApplicationPage
    {
        PivotItem allPivotItem;
        PivotItem searchResult;
        ProgressIndicator Pi;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the page DataContext property to the ViewModel.
            this.DataContext = App.ViewModel;

            Pi = new ProgressIndicator();
            Pi.IsIndeterminate = true;
            Pi.IsVisible = false;

            allPivotItem = new PivotItem();
            allPivotItem.Header = new ToDoCategory { Name = "All", IconPath = "/Sticky-Notes-icon.png" };
            ListBox listbox = new ListBox();
            listbox.Margin = new Thickness(12, 0, 12, 0);
            listbox.Width = 440;
            listbox.ItemTemplate = ToDoListBoxItemTemplate;
            listbox.Tap += Parent_Tap;
            allPivotItem.Content = listbox;

            searchResult = new PivotItem();
            searchResult.Header = new ToDoCategory { Name = "Search result", IconPath = "/Images/CategoryIcons/SearchResult.png" };
            ListBox lstbox = new ListBox();
            lstbox.Margin = new Thickness(12, 0, 12, 0);
            lstbox.Width = 440;
            lstbox.ItemTemplate = ToDoListBoxItemTemplate;
            lstbox.Tap += Parent_Tap;
            searchResult.Content = lstbox;

            App.ViewModel.OnDataBaseChange += ViewModel_onDataBaseChanged;
            App.ViewModel.ReLoadData();
            //TestData();
        }

        //void TestData()
        //{
        //    Random ran = new Random();
        //    for (int i = 0; i < 10; ++i)
        //    {
        //        int j = ran.Next() % 5;
        //        ToDoNote note = new ToDoNote { NoteTitle = "Note" + i.ToString(), Category = App.ViewModel.CategoriesList[j] , NoteDescription="This is note " + i.ToString()};
        //        App.ViewModel.AddToDoNote(note);
        //    }
        //}

        private void ShowProgressIndicator(string text)
        {
            Pi.IsIndeterminate = true;
            Pi.IsVisible = true;
            Pi.Text = text;
            SystemTray.SetProgressIndicator(this, Pi);

        }

        private void HideProgressIndicator()
        {
            Pi.IsIndeterminate = false;
            Pi.IsVisible = false;
            SystemTray.SetProgressIndicator(this, null);
        }

        void ViewModel_onDataBaseChanged(object sender, EventArgs e)
        {
            pivotControl.Items.Clear();
            ((ListBox)allPivotItem.Content).ItemsSource = App.ViewModel.AllToDoNotes.Collections;
            ((ListBox)searchResult.Content).ItemsSource = App.ViewModel.SearchResult.Collections;
            pivotControl.Items.Add(allPivotItem);
            int i;
            for (i = 0; i < App.ViewModel.CategoriesList.Count; ++i)
            {
                PivotItem pvitem = new PivotItem();
                pvitem.Header = App.ViewModel.CategoriesList[i];
                ListBox listbox = new ListBox();
                listbox.Margin = new Thickness(12, 0, 12, 0);
                listbox.Width = 440;
                listbox.ItemsSource = App.ViewModel.NoteCollectionList[i].Collections;
                listbox.ItemTemplate = ToDoListBoxItemTemplate;
                listbox.Tap += Parent_Tap;

                pvitem.Content = listbox;
                pivotControl.Items.Add(pvitem);
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Save changes to the database.
            App.ViewModel.SaveChangesToDB();
        }

        private void deleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            ToDoNote note = (ToDoNote)((Button)sender).DataContext;
            App.ViewModel.DeleteToDoNote(note);
        }

        private void btnAddNote_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CreateNotePage.xaml", UriKind.Relative));
        }

        private void btnSearchNote_Click(object sender, EventArgs e)
        {
            SlideSearchOn.Begin();
            txtPatern.Focus();
        }

        private void Parent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = false;
        }

        private void Parent_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = false;
        }

        private void Parent_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = false;
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.ViewModel.SelectedNote = (ToDoNote)((TextBlock)sender).DataContext;
            NavigationService.Navigate(new Uri("/NoteDetailPage.xaml", UriKind.Relative));
        }

        int _checked = 0;
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).Background = new SolidColorBrush(ColorConverter.MyColorTemplate);
            (sender as CheckBox).BorderBrush = new SolidColorBrush(Colors.Purple);
            _checked++;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).Background = new SolidColorBrush(Colors.White);
            (sender as CheckBox).BorderBrush = new SolidColorBrush(ColorConverter.MyColorTemplate);
            _checked--;
            if (_checked == 0)
                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = false;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtPatern.Text.Length > 0)
            {
                ShowProgressIndicator("Searching...");
                this.Focus();
                App.ViewModel.SearchNotes(txtPatern.Text);
                HideProgressIndicator();
                if (App.ViewModel.SearchResult.Collections.Count > 0)
                {
                    pivotControl.Items.Add(searchResult);
                    pivotControl.SelectedItem = searchResult;
                }
                else
                {
                    MessageBox.Show("No result found.");
                }
            }
        }

        private void txtPatern_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                btnSearch_Click(null, null);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            App.ViewModel.ReLoadData();
        }

        private void txtPatern_LostFocus(object sender, RoutedEventArgs e)
        {
            SlideSearchOff.Begin();
        }

        private void menuitemCheckall_Click(object sender, EventArgs e)
        {
            App.ViewModel.CheckAll();
        }

        private void menuitemUncheckall_Click(object sender, EventArgs e)
        {
            App.ViewModel.UnCheckAll();
        }

        private void menuitemDeleteall_Click(object sender, EventArgs e)
        {
            App.ViewModel.DeleteAllNote();
        }

        private void btnViewOnMap_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NoteMapViewPage.xaml", UriKind.Relative));
        }
    }
}