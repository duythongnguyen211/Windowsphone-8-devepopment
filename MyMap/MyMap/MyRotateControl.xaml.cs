using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;

namespace MyMap
{
    public partial class MyRotateControl : UserControl
    {
        public event EventHandler RotateControl_MouseMove;
        private bool mouseenter = false;
        private Point ePoint;
        public MyRotateControl()
        {
            InitializeComponent();
            AddElementToGrid(this.Height);
        }

        private void AddElementToGrid(double size)
        {
            InitializeComponent();
            Polygon north = new Polygon();
            north.Points.Add(new Point(size/2, 0));
            north.Points.Add(new Point(size / 2 - size / 10, size / 2));
            north.Points.Add(new Point(size / 2 + size / 10, size / 2));
            north.Fill = new SolidColorBrush(Colors.Red);
            north.Stroke = new SolidColorBrush(Colors.Black);
            north.Opacity = 100;

            Polygon south = new Polygon();
            south.Points.Add(new Point(size / 2 - size / 10, size / 2));
            south.Points.Add(new Point(size / 2, size));
            south.Points.Add(new Point(size / 2 + size / 10, size / 2));
            south.Stroke = new SolidColorBrush(Colors.Black);
            south.Fill = new SolidColorBrush(Colors.White);
            south.Opacity = 100;

            LayoutRoot.Children.Add(north);
            LayoutRoot.Children.Add(south);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Height = this.Width = this.Height < this.Width ? this.Height : this.Width;
            LayoutRoot.Children.Clear();
            AddElementToGrid(this.Height);
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseenter = true;
            ePoint = e.GetPosition(this);
        }

        private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mouseenter)
            {
                Point _newPoint = e.GetPosition(this);
                Point root = new Point(this.Height / 2, this.Height / 2);

                double AB, BC, CA;
                AB = Math.Sqrt(Math.Pow(ePoint.X - root.X, 2) + Math.Pow(ePoint.Y - root.Y, 2));
                BC = Math.Sqrt(Math.Pow(root.X - _newPoint.X, 2) + Math.Pow(root.Y - _newPoint.Y, 2));
                CA = Math.Sqrt(Math.Pow(_newPoint.X - ePoint.X, 2) + Math.Pow(_newPoint.Y - ePoint.Y, 2));
                double cosB = (AB * AB + BC * BC - CA * CA) / (2 * AB * BC);
                double _newangle = Math.Acos(cosB);
                double _oldangle = (double)this.RenderTransform.GetValue(CompositeTransform.RotationProperty);
                double error = _newPoint.Y - root.Y - ((ePoint.Y - root.Y)/(ePoint.X - root.X))*(_newPoint.X - root.X);
                if ( error > 0)
                    this.RenderTransform.SetValue(CompositeTransform.RotationProperty, (double)_oldangle + _newangle);
                else if (error < 0)
                    this.RenderTransform.SetValue(CompositeTransform.RotationProperty, (double)_oldangle - _newangle);
                if (RotateControl_MouseMove != null)
                    RotateControl_MouseMove(null, null);
                ePoint = _newPoint;
            }
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            mouseenter = false;
        }
    }
}
