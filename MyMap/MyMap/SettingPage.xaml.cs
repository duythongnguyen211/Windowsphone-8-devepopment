using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;

namespace MyMap
{
    public partial class SettingPage : PhoneApplicationPage
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            switch (MapProperties.mapMode)
            {
                case MapCartographicMode.Road: rdbtRoadMode.IsChecked = true; break;
                case MapCartographicMode.Aerial: rdbtAerialMode.IsChecked = true; break;
                case MapCartographicMode.Hybrid: rdbtHybridMode.IsChecked = true; break;
                case MapCartographicMode.Terrain: rdbtTerrainMode.IsChecked = true; break;
            }
            if (MapProperties.mapColorMode == MapColorMode.Light)
                rdbtLightMode.IsChecked = true;
            else rdbtDarkMode.IsChecked = true;
            cbLandmarksEnabled.IsChecked = MapProperties.LandmarksEnabled;
            cbATM.IsChecked = PushpinLayer.ATM;
            cbBuses.IsChecked = PushpinLayer.Buses;
            cbFavoriteplace.IsChecked = PushpinLayer.MyFavorite;
            cbHotel.IsChecked = PushpinLayer.Hotel;
            cbRestaurant.IsChecked = PushpinLayer.Restaurant;
            cbTourist.IsChecked = PushpinLayer.Tourist;
            base.OnNavigatedTo(e);
        }

        private void CbClick(object sender, RoutedEventArgs e)
        {
            CheckBox obj = (CheckBox)sender;
            switch((string)obj.Tag)
            {
                case "Fav": PushpinLayer.MyFavorite = (bool)obj.IsChecked; break;
                case "Res": PushpinLayer.Restaurant = (bool)obj.IsChecked; break;
                case "Hotel": PushpinLayer.Hotel = (bool)obj.IsChecked; break;
                case "Tourist": PushpinLayer.Tourist = (bool)obj.IsChecked; break;
                case "Buses": PushpinLayer.Buses = (bool)obj.IsChecked; break;
                case "ATM": PushpinLayer.ATM = (bool)obj.IsChecked; break;
                case "Landmarks": MapProperties.LandmarksEnabled = (bool)obj.IsChecked; break;
            }
        }

        private void RdbtChecked(object sender, RoutedEventArgs e)
        {
            switch((string)((RadioButton)sender).Tag)
            {
                case "Road": MapProperties.mapMode = MapCartographicMode.Road; break;
                case "Aerial": MapProperties.mapMode = MapCartographicMode.Aerial; break;
                case "Hybrid": MapProperties.mapMode = MapCartographicMode.Hybrid; break;
                case "Terrain": MapProperties.mapMode = MapCartographicMode.Terrain; break;
                case "Light": MapProperties.mapColorMode = MapColorMode.Light; break;
                case "Dark": MapProperties.mapColorMode = MapColorMode.Dark; break;
            }
        }
    }
}