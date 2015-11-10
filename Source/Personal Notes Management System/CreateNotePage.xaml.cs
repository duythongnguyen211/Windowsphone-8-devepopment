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

namespace Personal_Notes_Management_System
{
    public partial class CreateNotePage : PhoneApplicationPage
    {
        ProgressIndicator Pi;

        public CreateNotePage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
            categoriesListPicker.SelectedItem = App.ViewModel.CategoriesList[0];
            Pi = new ProgressIndicator();
            Pi.IsIndeterminate = true;
            Pi.IsVisible = false;

        }

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Return to the main page.
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void btnSaveNote_Click(object sender, EventArgs e)
        {
            // Confirm there is some text in the text box.
            if (txtNoteTitle.Text.Length > 0)
            {
                ShowProgressIndicator("Saving note...");
                // Create a new to-do item.
                ToDoNote newToDoNote = new ToDoNote
                {
                    NoteTitle = txtNoteTitle.Text,
                    Category = (ToDoCategory)categoriesListPicker.SelectedItem
                };

                // Add the item to the ViewModel.
                App.ViewModel.AddToDoNote(newToDoNote);
                HideProgressIndicator();
                // Return to the main page.
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            else MessageBox.Show("The title of note can not be empty.");
        }

        private void txtNoteTitle_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txtNoteTitle_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                this.Focus();
        }
    }
}