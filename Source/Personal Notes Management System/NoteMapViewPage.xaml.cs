using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using System.Windows.Media.Imaging;
using Personal_Notes_Management_System.Model;
using Personal_Notes_Management_System.XMLParser;
using Microsoft.Phone.Tasks;

namespace Personal_Notes_Management_System
{
    public partial class NoteMapViewPage : PhoneApplicationPage
    {
        GeoCoordinate myLocation;
        MapLayer myLocationLayer;
        List<MapLayer> categoryLayerList;
        MyPushpinControl mypushLocation;

        ProgressIndicator Pi;

        public NoteMapViewPage()
        {
            InitializeComponent();
            Pi = new ProgressIndicator();
            Pi.IsIndeterminate = true;
            Pi.IsVisible = false;
            myLocationLayer = new MapLayer();
            MyMap.Layers.Add(myLocationLayer);
            mypushLocation = new MyPushpinControl();
            mypushLocation.ImageSource = new BitmapImage(new Uri("/Assets/Pushpin/me-pushpin.png", UriKind.Relative));
            mypushLocation.Height = mypushLocation.Width = 30;
            this.DataContext = App.ViewModel;
            categoryLayerList = new List<MapLayer>();
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

        private void LoadNoteView(out int note_no_location_count)
        {
            note_no_location_count = 0;
            MyMap.Layers.Clear();
            MyMap.Layers.Add(myLocationLayer);
            categoryLayerList.Clear();

            for (int i = 0; i < App.ViewModel.CategoriesList.Count; ++i )
            {
                if (!App.ViewModel.CategoriesList[i].IsShowOnMap)
                    continue;
                MapLayer categorylayer = new MapLayer();
                foreach (ToDoNote note in App.ViewModel.AllToDoNotes.Collections)
                {
                    if (note.Category.Id != App.ViewModel.CategoriesList[i].Id)
                        continue;
                    if (note.NoteLocation != null && note.NoteLocation.Length > 0)
                    {
                        GeoCoordinate geocoodinate = LocationXmlParser.Parse(note.NoteLocation).GeoCoordinate;
                        NotePushpinControl pushpin = new NotePushpinControl(note, App.ViewModel.CategoriesList[i].IconSource, note.NoteTitle);
                        pushpin.PushpinTap += pushpin_PushpinTap;
                        ShowGeocoordinate(categorylayer, geocoodinate, pushpin);
                    }
                    else note_no_location_count++;
                }
                categoryLayerList.Add(categorylayer);
                MyMap.Layers.Add(categorylayer);
            }
        }

        void pushpin_PushpinTap(object sender, EventArgs e)
        {
            App.ViewModel.SelectedNote = (ToDoNote)((NotePushpinControl)sender).DataContext;
            NavigationService.Navigate(new Uri("/NoteDetailPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //load note
            int count;
            LoadNoteView(out count);
            if (e.NavigationMode == NavigationMode.New)
                if (count > 0)
                    MessageBox.Show("There are " + count.ToString() + " note(s) which is not  linked with a specific geographical location.");

        }

        private async void GetMyCoordinates()
        {
            ShowProgressIndicator("Getting your location...");
            // Get the phone's current location.
            Geolocator MyGeolocator = new Geolocator();
            MyGeolocator.DesiredAccuracyInMeters = 5;
            Geoposition MyGeoPosition = null;
            try
            {
                MyGeoPosition = await MyGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                myLocation = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
                myLocationLayer.Clear();
                ShowGeocoordinate(myLocationLayer, myLocation, mypushLocation);
                MyMap.SetView(myLocation, 17, MapAnimationKind.Parabolic);
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (Exception)
            {
            }
            HideProgressIndicator();
        }

        private int ShowGeocoordinate(MapLayer layer, GeoCoordinate p, object pushpin)
        {
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = pushpin;
            myLocationOverlay.PositionOrigin = new Point(0, 1);
            myLocationOverlay.GeoCoordinate = p;
            layer.Add(myLocationOverlay);
            return myLocationLayer.Count - 1;
        }

        private void MyMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Point p = e.GetPosition(MyMap);
            MyMap.SetView(MyMap.ConvertViewportPointToGeoCoordinate(p), MyMap.ZoomLevel, MapAnimationKind.Parabolic);
        }

        private void slider_Pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyMap.Pitch = slider_Pitch.Value;
        }

        private void btnRotate_Click(object sender, RoutedEventArgs e)
        {
            MyMap.Heading += 10;
        }

        private void btnMe_Click(object sender, EventArgs e)
        {
            this.GetMyCoordinates();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryLayerSettingPage.xaml", UriKind.Relative));
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            MapDownloaderTask task = new MapDownloaderTask();
            task.Show();
        }
    }
}