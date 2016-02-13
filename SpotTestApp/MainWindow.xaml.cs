using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<SolidColorBrush> brushes = new List<SolidColorBrush>()
        {
            Brushes.Gray,
            Brushes.Black,
            Brushes.CadetBlue,
            Brushes.Olive,
            Brushes.Orange,
            Brushes.Maroon,
            Brushes.LightYellow,
            Brushes.DarkGreen
        };

        public MainWindow()
        {
            InitializeComponent();
            cbColors.ItemsSource = brushes;
            cbGridColor.ItemsSource = brushes;
            cbFunctions.ItemsSource = Enum.GetValues(typeof(Functions));
            applyChanges();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = tbName.Text;
                Functions function = (Functions)cbFunctions.SelectedItem;
                double left = double.Parse(tbLeftBound.Text);
                double right = double.Parse(tbRightBound.Text);
                int n = int.Parse(tbN.Text);
                int thickness = int.Parse(tbLineWidth.Text);
                SolidColorBrush color = (SolidColorBrush)cbColors.SelectedItem;

                var points = getPoints(function, left, right, n);
                spot.AddGraph(points, color, thickness, name);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went bad. Details: \n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbGraphs.SelectedItem != null)
            {
                string name = ((Polyline)lbGraphs.SelectedItem).Name;
                try
                {
                    spot.RemoveGraph(name);
                    spot.Update();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error. Details: \n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private List<Point> getPoints(Functions function, double a, double b, int n)
        {
            List<Point> points = new List<Point>();
            double h = (b - a) / n;
            for (double x = a; x <= b; x += h)
            {
                double y;
                switch (function)
                {
                    case Functions.Cos:
                        y = Math.Cos(x);
                        break;
                    case Functions.Ln:
                        y = Math.Log(x);
                        break;
                    case Functions.Sin:
                        y = Math.Sin(x);
                        break;
                    case Functions.Sqr:
                        y = x * x;
                        break;
                    case Functions.Sqrt:
                        y = Math.Sqrt(x);
                        break;
                    case Functions.x:
                        y = x;
                        break;
                    default:
                        y = 0;
                        break;
                }

                points.Add(new Point(x, y));
            }
            return points;
        }

        private void btnApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            applyChanges();
        }

        private void applyChanges()
        {
            string header = tbHeader.Text;
            int xSize = int.Parse(tbGridXSize.Text);
            int ySize = int.Parse(tbGridYSize.Text);
            string xAxis = "x";
            string yAxis = "f(x)";
            SolidColorBrush gridColor = (SolidColorBrush)cbGridColor.SelectedItem;
            bool showInfo = cbShowInfo.IsChecked.Value;

            spot.SpotName = header;
            spot.ShowGraphInfo = showInfo;
            spot.GridMinWidth = xSize;
            spot.GridMinHeight = ySize;
            spot.GridColor = gridColor;
            spot.HorizontalAxisName = xAxis;
            spot.VerticalAxisName = yAxis;
            spot.Update();
        }

        private void btnSaveAsImage_Click(object sender, RoutedEventArgs e)
        {
            spot.SaveAsImage("result.png", SpotLibrary.PlotImageFormat.png);
            spot.SaveAsImage("result.bmp", SpotLibrary.PlotImageFormat.bmp);
            spot.SaveAsImage("result.jpg", SpotLibrary.PlotImageFormat.jpg);
        }
    }

    public enum Functions { x, Sqr, Sqrt, Sin, Cos, Ln }
}
