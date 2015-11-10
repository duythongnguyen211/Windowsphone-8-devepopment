using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Windows.Devices.Geolocation;

namespace Personal_Notes_Management_System
{
    public partial class SelectLocationPage : PhoneApplicationPage
    {
        GeoCoordinate selectedGeocoordinate, myLocation;
        string selectedAddress;
        ProgressIndicator Pi;

        public SelectLocationPage()
        {
            InitializeComponent();
            ctrlSearch.Width = 0;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("geocoordinate"))
            {
                GeoCoordinate geo = (GeoCoordinate)PhoneApplicationService.Current.State["geocoordinate"];
                ShowGeocoordinate(geo);
            }
            else GetMyCoordinates();
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
                ShowGeocoordinate(myLocation);
                selectedGeocoordinate = myLocation;
                selectedAddress = "";
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (Exception)
            {
            }
            HideProgressIndicator();
        }

        private void MyMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MyMap.Layers.Clear();
            Point p = e.GetPosition(MyMap);
            selectedGeocoordinate = MyMap.ConvertViewportPointToGeoCoordinate(p);
            ShowGeocoordinate(selectedGeocoordinate);
            selectedAddress = "";
        }

        private void ShowGeocoordinate(GeoCoordinate p)
        {
            MyPushpinControl pushpin = new MyPushpinControl();
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = pushpin;
            myLocationOverlay.PositionOrigin = new Point(0.5, 1);
            myLocationOverlay.GeoCoordinate = p;

            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);

            MyMap.Layers.Add(myLocationLayer);
            MyMap.SetView(p, 17, MapAnimationKind.Parabolic);
        }

        private void slider_Pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyMap.Pitch = slider_Pitch.Value;
        }

        private void btnRotate_Click(object sender, RoutedEventArgs e)
        {
            MyMap.Heading += 10;
        }

        private void ctrlSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ShowProgressIndicator("Searching...");
                GeocodeQuery myGeocodequery = new GeocodeQuery();
                myGeocodequery.SearchTerm = ctrlSearch.Text;
                myGeocodequery.GeoCoordinate = new GeoCoordinate(10, 10);
                myGeocodequery.QueryCompleted += myGeocodequery_QueryCompleted;
                myGeocodequery.QueryAsync();
            }
        }

        List<MapLocation> infoSearchresult;
        private void myGeocodequery_QueryCompleted(object sender,
        QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            HideProgressIndicator();
            if (e.Error == null && e.Result.Count > 0)
            {
                infoSearchresult = e.Result.ToList();
                listResult.Visibility = System.Windows.Visibility.Visible;
                listResult.Items.Clear();

                for (int i = 0; i < e.Result.Count; ++i)
                {
                    TextBlock text = new TextBlock();
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Text = GetAddress(e.Result[i].Information.Address);
                    text.Tap += text_Tap;
                    listResult.Items.Add(text);
                }
                listResult.Height = 160;
            }
            else
            {
                listResult.Visibility = System.Windows.Visibility.Collapsed;
                MessageBox.Show("Can not search your address.");
            }
        }

        private string GetAddress(MapAddress address)
        {
            string result = "";
            if (address.HouseNumber != "")
                result += address.HouseNumber + ", ";
            if (address.Street != "")
                result += address.Street + ", ";
            if (address.County != "")
                result += address.County + ", ";
            if (address.District != "")
                result += address.District + ", ";
            if (address.City != "")
                result += address.City + ", ";
            if (address.Country != "")
                result += address.Country;
            return result;
        }

        void text_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MyMap.Layers.Clear();
            int index = listResult.SelectedIndex;
            selectedGeocoordinate = infoSearchresult[index].GeoCoordinate;
            selectedAddress = GetAddress(infoSearchresult[index].Information.Address);
            ShowGeocoordinate(selectedGeocoordinate);
        }

        private void listResult_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = false;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (selectedAddress == "")
            {
                ShowProgressIndicator("Getting address...");
                ReverseGeocodeQuery query = new ReverseGeocodeQuery()
                {
                    GeoCoordinate = selectedGeocoordinate
                };
                query.QueryCompleted += query_QueryCompleted;
                query.QueryAsync();

            }
            else
            {
                PhoneApplicationService.Current.State["location"] = selectedGeocoordinate;
                PhoneApplicationService.Current.State["address"] = selectedAddress;
                NavigationService.GoBack();
            }
        }

        void query_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            HideProgressIndicator();
            if (e.Result != null && e.Result.Count > 0)
            {
                selectedAddress = GetAddress(e.Result[0].Information.Address);
            }
            else selectedAddress = "Can not found address";
            PhoneApplicationService.Current.State["location"] = selectedGeocoordinate;
            PhoneApplicationService.Current.State["address"] = selectedAddress;
            NavigationService.GoBack();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            listResult.Visibility = System.Windows.Visibility.Collapsed;
            SlideSearchOn.Begin();
            ctrlSearch.TextboxFocus();
        }

        private void btnMe_Click(object sender, EventArgs e)
        {
            listResult.Visibility = System.Windows.Visibility.Collapsed;
            GetMyCoordinates();
        }
    }
}