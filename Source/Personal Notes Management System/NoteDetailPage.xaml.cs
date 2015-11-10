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
using Personal_Notes_Management_System.XMLParser;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using Personal_Notes_Management_System.Storage;
using Microsoft.Phone.Scheduler;
using System.Device.Location;

namespace Personal_Notes_Management_System
{
    public partial class NoteDetailPage : PhoneApplicationPage
    {
        private ObservableCollection<XMLParser.MediaElement> photos = new ObservableCollection<XMLParser.MediaElement>();    //path
        private ObservableCollection<XMLParser.MediaElement> voicespath = new ObservableCollection<XMLParser.MediaElement>();    //path
        ApplicationBarIconButton buttonAdd;
        ApplicationBarIconButton buttonDelete;
        MyLocation mylocation;
        ProgressIndicator Pi;

        public NoteDetailPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
            Pi = new ProgressIndicator();
            Pi.IsIndeterminate = true;
            Pi.IsVisible = false;
            mylocation = new MyLocation();
            LoadNote(App.ViewModel.SelectedNote);
            buttonAdd = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            buttonDelete = (ApplicationBarIconButton)ApplicationBar.Buttons[3];

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

        protected void LoadNote(ToDoNote note)
        {
            ShowProgressIndicator("Loading note's detail...");
            if (note != null)
            {
                //title
                noteTitle.Text = note.NoteTitle;
                txtTitle.Text = note.NoteTitle;

                // description
                if (note.NoteDescription != null)
                    txtDescription.Text = note.NoteDescription;
                else txtDescription.Text = "";

                //tags
                if (note.NoteTags != null)
                {
                    listTags.Visibility = System.Windows.Visibility.Visible;
                    txtTags.Visibility = System.Windows.Visibility.Collapsed;
                    listTags.Items.Clear();
                    txtTags.Text = note.NoteTags;
                    foreach (string tag in note.NoteTags.Split(new char[] { ' ' }))
                    {
                        Button button = new Button();
                        button.Click += button_Click;
                        button.Content = tag;
                        button.Background = new SolidColorBrush(ColorConverter.MyColorTemplate);
                        button.Height = 72;
                        button.Width = 50 + tag.Length * 14;

                        listTags.Items.Add(button);
                    }
                }
                else
                {
                    listTags.Visibility = System.Windows.Visibility.Collapsed;
                    txtTags.Visibility = System.Windows.Visibility.Visible;
                    txtTags.Text = "";
                    listTags.Items.Clear();
                }

                //category
                App.ViewModel.CategorySelected = note.Category;

                //location
                mylocation = LocationXmlParser.Parse(note.NoteLocation);
                if (mylocation != null)
                    txtLocation.Text = mylocation.Address;
                else txtLocation.Text = "";

                // photos
                if (note.NotePhotos == null || note.NotePhotos.Length <= 0) ;
                else
                {
                    photos = new ObservableCollection<XMLParser.MediaElement>(MediaXmlParser.Parse(note.NotePhotos));
                }
                listphoto.ItemsSource = photos;

                //voices
                if (note.NoteVoices == null || note.NoteVoices.Length <= 0) ;
                else
                {
                    voicespath = new ObservableCollection<XMLParser.MediaElement>(MediaXmlParser.Parse(note.NoteVoices));
                }
                listvoice.ItemsSource = voicespath;

                //reminder
                checkboxtime.IsChecked = note.IsTimeReminder;
                checkboxdis.IsChecked = note.IsDistanceReminder;

                if (note.TimeReminder != null)
                {
                    datepicker.Value = DateTime.Parse(note.TimeReminder.ToString());
                    timepicker.Value = DateTime.Parse(note.TimeReminder.ToString());
                }

                txtDistance.Text = note.NoteDistanceReminder.ToString();
            }
            HideProgressIndicator();
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnSaveChange_Click(object sender, EventArgs e)
        {
            ShowProgressIndicator("Saving changes...");
            //reminder
            if (checkboxtime.IsChecked == true)
            {
                DateTime _datetime = (DateTime)datepicker.Value + ((DateTime)timepicker.Value).TimeOfDay;
                if (_datetime <= DateTime.Now)
                {
                    MessageBox.Show("The datetime must be in the future.");
                    goto Label_0211;
                }

                App.ViewModel.SelectedNote.TimeReminder = _datetime;
            }
            if (checkboxdis.IsChecked == true)
                try
                {
                    App.ViewModel.SelectedNote.NoteDistanceReminder = double.Parse(txtDistance.Text.Trim());
                }
                catch (Exception)
                {
                    App.ViewModel.SelectedNote.NoteDistanceReminder = 0.0f;
                }
            App.ViewModel.SelectedNote.IsTimeReminder = checkboxtime.IsChecked == true ? true : false;
            App.ViewModel.SelectedNote.IsDistanceReminder = checkboxdis.IsChecked == true ? true : false;

            // title
            if (txtTitle.Text.Length > 0)
                App.ViewModel.SelectedNote.NoteTitle = txtTitle.Text;
            else
            {
                MessageBox.Show("The title can not be empty.");
                goto Label_0211;
            }

            //description
            App.ViewModel.SelectedNote.NoteDescription = txtDescription.Text;
            //tags
            App.ViewModel.SelectedNote.NoteTags = txtTags.Text;
            //category
            App.ViewModel.SelectedNote.Category = categoriesListPicker.SelectedItem as ToDoCategory;
            //location
            App.ViewModel.SelectedNote.NoteLocation = LocationXmlParser.CreateXmlString(mylocation);
            ////photos
            App.ViewModel.SelectedNote.NotePhotos = XMLParser.MediaXmlParser.CreateXmlString(photos.ToList(), "photos");
            ////voices
            App.ViewModel.SelectedNote.NoteVoices = XMLParser.MediaXmlParser.CreateXmlString(voicespath.ToList(), "voices");
            App.ViewModel.SaveChangesToDB();

            ScheduledAction currentReminder = ScheduledActionService.Find(App.ViewModel.SelectedNote.ToDoNoteId.ToString());

            if (currentReminder != null)
            {
                ScheduledActionService.Remove(currentReminder.Name);
            }

            if (App.ViewModel.SelectedNote.IsTimeReminder)
            {
                var reminder = new Microsoft.Phone.Scheduler.Reminder(App.ViewModel.SelectedNote.ToDoNoteId.ToString())
                {
                    BeginTime = (DateTime)App.ViewModel.SelectedNote.TimeReminder,
                    Title = App.ViewModel.SelectedNote.NoteTitle,
                    Content = App.ViewModel.SelectedNote.NoteDescription,
                };

                ScheduledActionService.Add(reminder);
            }
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        Label_0211:
            HideProgressIndicator();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void txtTags_LostFocus(object sender, RoutedEventArgs e)
        {
            txtTags.Visibility = System.Windows.Visibility.Collapsed;
            listTags.Visibility = System.Windows.Visibility.Visible;
            listTags.Items.Clear();
            foreach (string tag in txtTags.Text.Trim().Split(new char[] { ' ' }))
            {
                Button button = new Button();
                button.Click += button_Click;
                button.Content = tag;
                button.Background = new SolidColorBrush(ColorConverter.MyColorTemplate);
                button.Height = 72;
                button.Width = 50 + tag.Length * 14;

                listTags.Items.Add(button);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).BorderBrush = new SolidColorBrush(Colors.Purple);
            (sender as CheckBox).Background = new SolidColorBrush(ColorConverter.MyColorTemplate);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).BorderBrush = new SolidColorBrush(ColorConverter.MyColorTemplate);
            (sender as CheckBox).Background = new SolidColorBrush(Colors.White);
        }

        private void listTags_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            listTags.Visibility = System.Windows.Visibility.Collapsed;
            txtTags.Visibility = System.Windows.Visibility.Visible;
            txtTags.Focus();
        }

        private void Parent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = false;
        }

        private void txtDistance_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key >= System.Windows.Input.Key.D0 && e.Key <= System.Windows.Input.Key.D9) || e.Key == System.Windows.Input.Key.Back)
                return;
            if (e.Key == System.Windows.Input.Key.Enter)
                this.Focus();
            e.Handled = true;
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                this.Focus();
        }


        List<XMLParser.MediaElement> photosselected = new List<XMLParser.MediaElement>();   //path
        List<XMLParser.MediaElement> voicesselected = new List<XMLParser.MediaElement>();   //name

        private void photocheckbox_Checked(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).BorderBrush = new SolidColorBrush(Colors.Purple);
            (sender as CheckBox).Background = new SolidColorBrush(ColorConverter.MyColorTemplate);
            photosselected.Add((XMLParser.MediaElement)(sender as CheckBox).DataContext);
            buttonDelete.IsEnabled = true;
        }

        private void photocheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).BorderBrush = new SolidColorBrush(ColorConverter.MyColorTemplate);
            (sender as CheckBox).Background = new SolidColorBrush(Colors.White);
            photosselected.Remove((XMLParser.MediaElement)(sender as CheckBox).DataContext);
            if (photosselected.Count <= 0)
                buttonDelete.IsEnabled = false;
        }

        private void voicecheckbox_Checked(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).BorderBrush = new SolidColorBrush(Colors.Purple);
            (sender as CheckBox).Background = new SolidColorBrush(ColorConverter.MyColorTemplate);
            voicesselected.Add((XMLParser.MediaElement)(sender as CheckBox).DataContext);
            buttonDelete.IsEnabled = true;
        }

        private void voicecheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).BorderBrush = new SolidColorBrush(ColorConverter.MyColorTemplate);
            (sender as CheckBox).Background = new SolidColorBrush(Colors.White);
            voicesselected.Remove((XMLParser.MediaElement)(sender as CheckBox).DataContext);
            if (voicesselected.Count <= 0)
                buttonDelete.IsEnabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (pivotcontrol.SelectedIndex == 1)
            {
                PhotoChooserTask photoChooserTask = new PhotoChooserTask();
                photoChooserTask.ShowCamera = true;
                photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
                photoChooserTask.Show();
            }
            else if (pivotcontrol.SelectedIndex == 2)
            {
            }
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                string fname = "/SavedImage/MyImage_" + photos.Count.ToString() + ".jpg";
                BitmapImage img = new BitmapImage();
                img.SetSource(e.ChosenPhoto);
                if (img != null)
                {
                    StorageManager.CreateDirectory("SavedImage");
                    StorageManager.SaveImage(img, fname);
                    photos.Add(new XMLParser.MediaElement(fname));
                }
            }
        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            if (pivotcontrol.SelectedIndex == 1)
            {
                IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
                foreach (XMLParser.MediaElement path in photosselected)
                {
                    photos.Remove(path);
                    if (isf.FileExists(path.FullPath))
                        isf.DeleteFile(path.FullPath);
                }
                photosselected.Clear();
            }
            else if (pivotcontrol.SelectedIndex == 2)
            {

            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pivotcontrol.SelectedIndex == 1)
            {
                buttonAdd.IsEnabled = true;
                if (photosselected != null && photosselected.Count > 0)
                    buttonDelete.IsEnabled = true;
            }
            else if (pivotcontrol.SelectedIndex == 2)
            {
                buttonAdd.IsEnabled = true;
                if (voicesselected != null && voicesselected.Count > 0)
                    buttonDelete.IsEnabled = true;
            }
            else
            {
                buttonAdd.IsEnabled = false;
            }
        }

        private void btnViewMap_Click(object sender, RoutedEventArgs e)
        {
            if (mylocation != null)
                PhoneApplicationService.Current.State["geocoordinate"] = mylocation.GeoCoordinate;
            NavigationService.Navigate(new Uri("/SelectLocationPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("location"))
            {
                mylocation = new MyLocation();
                mylocation.GeoCoordinate = (GeoCoordinate)PhoneApplicationService.Current.State["location"];
                mylocation.Address = (string)PhoneApplicationService.Current.State["address"];
                txtLocation.Text = mylocation.Address;
                PhoneApplicationService.Current.State.Clear();
            }
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            App.ViewModel.DeleteToDoNote(App.ViewModel.SelectedNote);
            NavigationService.GoBack();
        }
    }
}