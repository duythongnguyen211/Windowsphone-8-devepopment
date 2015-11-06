using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;
using Microsoft.Xna.Framework.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Windows.Devices.Sensors;

namespace MyMap
{
    public partial class PhotoCameraPage : PhoneApplicationPage
    {
        private int photoCounter = 0;
        PhotoCamera cam;
        MediaLibrary library = new MediaLibrary();
        Compass compass;

        public PhotoCameraPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo
          (System.Windows.Navigation.NavigationEventArgs e)
        {
            if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true)
            {
                cam = new PhotoCamera(CameraType.Primary);
                cam.CaptureImageAvailable +=
                  new EventHandler<Microsoft.Devices.ContentReadyEventArgs>
                    (cam_CaptureImageAvailable);
                viewfinderBrush.SetSource(cam);
            }
            else
            {
                txtMessage.Text = "A Camera is not available on this device.";
            }
        }
        protected override void OnNavigatingFrom
          (System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                cam.Dispose();
            }
        }

        void viewfinder_Tapped(object sender, GestureEventArgs e)
        {
            if (cam != null)
            {
                try
                {
                    //cam.CaptureImage();
                    Image hehe = new Image();
                    hehe.Height = 76;
                    hehe.Width = 63;
                    hehe.Source = PushpinImageSource.Restaurant;
                    hehe.SetValue(Canvas.LeftProperty, e.GetPosition(viewfinderCanvas).X - hehe.Width/2);
                    hehe.SetValue(Canvas.TopProperty, e.GetPosition(viewfinderCanvas).Y - hehe.Height);
                    viewfinderCanvas.Children.Add(hehe);
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        txtMessage.Text = ex.Message;
                    });
                }
            }
        }

        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            photoCounter++;
            string fileName = photoCounter + ".jpg";
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                txtMessage.Text = "Captured image available, saving picture.";
            });
            library.SavePictureToCameraRoll(fileName, e.ImageStream);
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                txtMessage.Text = "Picture has been saved to camera roll.";
            });
        }

        protected override void OnOrientationChanged
          (OrientationChangedEventArgs e)
        {
            if (cam != null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    double rotation = cam.Orientation;
                    switch (this.Orientation)
                    {
                        case PageOrientation.LandscapeLeft:
                            rotation = cam.Orientation - 90;
                            break;
                        case PageOrientation.LandscapeRight:
                            rotation = cam.Orientation + 90;
                            break;
                    }
                    viewfinderTransform.Rotation = rotation;
                });
            }
            base.OnOrientationChanged(e);
        }
    }
}