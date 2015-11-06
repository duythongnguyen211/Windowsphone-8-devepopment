using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Devices.Geolocation;
using Windows.Phone.Speech.Synthesis;
using System.Windows.Input;
using System.Threading;
using System.Windows.Media.Animation;

namespace MyMap
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinate myGeocoordinate;
        Geoposition myGeoposition;
        GeocodeQuery myGeocodequery;
        MapLayer myLocationLayer;
        MyPushpinControl myLocation;
        ProgressIndicator Pi;
        int index_of_layer_mylocation = -1;
        public MainPage()
        {
            InitializeComponent();
            ctrlSearch.Width = 0;
            myLocationLayer = new MapLayer();
            MyMap.Layers.Add(myLocationLayer);
            SetMapBegin();
            myLocation = new MyPushpinControl(PushpinImageSource.Me, pushpin_Tap, "This is your current location");
            myLocation.Height = myLocation.Width = 30;
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

        private void pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBox.Show(((MyPushpinControl)sender).Tag.ToString());
        }

        private async void GetMyLocation()
        {
            Geolocator MyGeolocator = new Geolocator();
            MyGeolocator.DesiredAccuracyInMeters = 5;
            myGeoposition = null;
            try
            {
                ShowProgressIndicator("Getting your location...");
                myGeoposition = await MyGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                myGeocoordinate = new GeoCoordinate(myGeoposition.Coordinate.Latitude, myGeoposition.Coordinate.Longitude);
                if (index_of_layer_mylocation < 0)
                    index_of_layer_mylocation = ShowGeocoordinate(myGeocoordinate, myLocation);
                else myLocationLayer.ElementAt(index_of_layer_mylocation).GeoCoordinate = myGeocoordinate;
                MyMap.SetView(myGeocoordinate, 17, MapAnimationKind.Parabolic);
                HideProgressIndicator();

            }
            catch (UnauthorizedAccessException)
            {
                HideProgressIndicator();
                MessageBox.Show("Location is disabled in phone settings or capabilities are not checked.");
            }
            catch (Exception ex)
            {
                HideProgressIndicator();
                MessageBox.Show(ex.Message);
            }
        }

        private void SetMapBegin()
        {
            MyMap.SetView(new GeoCoordinate(10, 106), 10, MapAnimationKind.Parabolic);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SlideSearchOn.Begin();
            ctrlSearch.TextboxFocus();
        }

        private void MyMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Point p = e.GetPosition(MyMap);
            GeoCoordinate s = MyMap.ConvertViewportPointToGeoCoordinate(p);
            //ReverseGeocodeQuery query = new ReverseGeocodeQuery()
            //{
            //    GeoCoordinate = s
            //};
            //query.QueryCompleted += query_QueryCompleted;
            //query.QueryAsync();
        }

        //void query_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        //{
        //    LocationInformation result = e.Result[0].Information;
        //}

        private void ZoomCtrl_ZoomCtrlInClick(object sender, EventArgs e)
        {
            if (MyMap.ZoomLevel <= 19)
                MyMap.ZoomLevel++;
            else MyMap.ZoomLevel = 20;
        }

        private void ZoomCtrl_ZoomCtrlOutClick(object sender, EventArgs e)
        {
            if (MyMap.ZoomLevel >= 2)
                MyMap.ZoomLevel--;
        }

        private int ShowGeocoordinate(GeoCoordinate p, object pushpin)
        {
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = pushpin;
            myLocationOverlay.PositionOrigin = new Point(0, 1);
            myLocationOverlay.GeoCoordinate = p;
            myLocationLayer.Add(myLocationOverlay);
            return myLocationLayer.Count - 1;
        }

        private void slider_Pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider_Pitch != null)
                MyMap.Pitch = slider_Pitch.Value;
        }

        private void ctrlSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                myGeocodequery = new GeocodeQuery();
                myGeocodequery.SearchTerm = ctrlSearch.Text;
                myGeocodequery.GeoCoordinate = MyMap.Center;
                myGeocodequery.QueryCompleted += myGeocodequery_QueryCompleted;
                ShowProgressIndicator("Searching...");
                myGeocodequery.QueryAsync();
                MyMap.Focus();
            }
        }

        private void myGeocodequery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            HideProgressIndicator();
            if (e.Error == null && e.Result.Count > 0)
            {
                for (int i = 0; i < e.Result.Count; ++i)
                {
                    ShowGeocoordinate(e.Result[i].GeoCoordinate, new MyPushpinControl(null, pushpin_Tap, GetAddress(e.Result[i].Information.Address)));
                    MyMap.SetView(e.Result[0].GeoCoordinate, 17, MapAnimationKind.Parabolic);
                }
            }
            else MessageBox.Show("Can not search your address.");
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml",UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyMap.CartographicMode = MapProperties.mapMode;
            MyMap.ColorMode = MapProperties.mapColorMode;
            MyMap.LandmarksEnabled = MapProperties.LandmarksEnabled;
            base.OnNavigatedTo(e);
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

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            MyMap.Layers.Clear();
            myLocationLayer = new MapLayer();
            MyMap.Layers.Add(myLocationLayer);
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            MyMap.SetView(new GeoCoordinate(0, 0), 1, MapAnimationKind.Parabolic);
        }

        private void btnMe_Click(object sender, EventArgs e)
        {
            this.GetMyLocation();
        }

        private void MyMap_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
        }

        private void btnRotate_Click(object sender, RoutedEventArgs e)
        {
            MyMap.Heading = (MyMap.Heading + 36)%360;
        }

        private void MyMap_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            //ctrlSearch.Visibility = System.Windows.Visibility.Visible;
            //ctrlSearch.Text = MyMap.ZoomLevel.ToString();
        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PhotoCameraPage.xaml", UriKind.Relative));
        }

        //List<GeoCoordinate> listService;
        //List<GeoCoordinate> listFilter;
        //GeoCoordinate myGeoCoorcinate;
        //public void ChonDiaDiemTrongBanKinhChoPhep(double limitDistance)
        //{
        //    listFilter = new List<GeoCoordinate>();
        //    foreach (GeoCoordinate pos in listService)
        //    {
        //        double distance = myGeoCoorcinate.GetDistanceTo(pos);
        //        if (distance <= limitDistance)
        //            listFilter.Add(pos);
        //    }
        //}

        //int? minLenght = null;
        //GeoCoordinate resultService;
        //public void ChonDiaDiemCoDuongDiNganNhat()
        //{
        //    foreach (GeoCoordinate pos in listFilter)
        //        GetRoute(pos);
        //}

        //private  void GetRoute(GeoCoordinate pos)
        //{
        //    RouteQuery MyQuery = new RouteQuery();
        //    List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
        //    MyCoordinates.Add(myGeoCoorcinate);
        //    MyCoordinates.Add(pos);
        //    MyQuery.Waypoints = MyCoordinates;
        //    MyQuery.QueryCompleted += MyQuery_QueryCompleted;
        //    MyQuery.QueryAsync();
        //}

        //private void MyQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        //{
        //    if (e.Error == null)
        //    {
        //        Route MyRoute = e.Result;
        //        if (minLenght == null || MyRoute.LengthInMeters < minLenght)
        //        {
        //            minLenght = MyRoute.LengthInMeters;
        //            resultService = ((RouteQuery)sender).Waypoints.ElementAt(1);
        //        }
        //    }
        //}
    }

    class MapProperties
    {
        public static MapCartographicMode mapMode = MapCartographicMode.Road;
        public static MapColorMode mapColorMode = MapColorMode.Light;
        public static bool LandmarksEnabled = false;
    }

    class PushpinLayer
    {
        public static bool MyFavorite = false;
        public static bool Restaurant = false;
        public static bool Hotel = false;
        public static bool Tourist = false;
        public static bool Buses = false;
        public static bool ATM = false;
    }

    class PushpinImageSource
    {
        public static readonly ImageSource Me = new BitmapImage(new Uri(@"/Assets/Pushpin/me-pushpin.png", UriKind.RelativeOrAbsolute));
        public static readonly ImageSource Default = new BitmapImage(new Uri(@"/Assets/Pushpin/default-pushpin.png", UriKind.RelativeOrAbsolute));
        public static readonly ImageSource Favorite = new BitmapImage(new Uri(@"/Assets/Pushpin/fav-pushpin.png", UriKind.RelativeOrAbsolute));
        public static readonly ImageSource Restaurant = new BitmapImage(new Uri(@"/Assets/Pushpin/res-pushpin.png", UriKind.RelativeOrAbsolute));
        public static readonly ImageSource Hotel = new BitmapImage(new Uri(@"/Assets/Pushpin/hotel-pushpin.png", UriKind.RelativeOrAbsolute));
        public static readonly ImageSource Tourist = new BitmapImage(new Uri(@"/Assets/Pushpin/tourist-pushpin.png", UriKind.RelativeOrAbsolute));
        public static readonly ImageSource Buses = new BitmapImage(new Uri(@"/Assets/Pushpin/bus-pushpin.png", UriKind.RelativeOrAbsolute));
        public static readonly ImageSource ATM = new BitmapImage(new Uri(@"/Assets/Pushpin/atm-pushpin.png", UriKind.RelativeOrAbsolute));
    }
}