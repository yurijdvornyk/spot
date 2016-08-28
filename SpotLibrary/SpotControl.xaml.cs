using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotLibrary
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SpotControl : UserControl
    {
        #region Fields and properties

        // Real canvas width and height
        protected double CanvasWidth { get; set; }
        protected double CanvasHeight { get; set; }

        // Bounds of all the graphs
        public double Xmin { get; set; }
        public double Ymin { get; set; }
        public double Xmax { get; set; }
        public double Ymax { get; set; }

        public Rect FixedBounds { get; protected set; }
        public bool IsFixedBound { get; protected set; }

        /// <summary>
        /// Graph name
        /// </summary>
        public string SpotName
        {
            get { return tbGraphName.Text; }
            set { tbGraphName.Text = value; }
        }

        /// <summary>
        /// Horizontal axis name
        /// </summary>
        public string HorizontalAxisName
        {
            get
            {
                return lblXAxisName.Content.ToString();
            }
            set
            {
                lblXAxisName.Content = value;
            }
        }

        /// <summary>
        /// Vertical axis name
        /// </summary>
        public string VerticalAxisName
        {
            get
            {
                return lblYAxisName.Content.ToString();
            }
            set
            {
                lblYAxisName.Content = value;
            }
        }

        // Minimum size of grid cell
        public int GridMinWidth { get; set; }
        public int GridMinHeight { get; set; }

        public bool ShowGrid { get; set; }
        public bool ShowGraphInfo
        {
            get
            {
                if (lvInfo.Visibility == Visibility.Visible) { return true; }
                else { return false; }
            }
            set
            {
                if (value == true) { lvInfo.Visibility = Visibility.Visible; }
                else { lvInfo.Visibility = Visibility.Hidden; }

                // Just to make Graphs collection update. Without it the list of files can't update dynamically.
                Graphs.Add(new Graph());
                Graphs.Remove(Graphs.Last());
            }
        }

        // Colors
        public SolidColorBrush GridColor { get; set; }
        //public SolidColorBrush GraphColor { get; set; }

        // Accuracy of numbers.
        public int DigitsAfterPoint { get; set; }

        /// <summary>
        /// Collection of graphs
        /// </summary>
        public ObservableCollection<Graph> Graphs { get; set; }

        #endregion

        #region Contructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SpotControl()
        {
            InitializeComponent();
            Graphs = new ObservableCollection<Graph>();
            ((INotifyCollectionChanged)lvInfo.Items).CollectionChanged += lvInfo_CollectionChanged;
            ShowGrid = true;
            GridColor = Brushes.Gray;
            MainBorder.Stroke = GridColor;
            GridMinWidth = 60;
            GridMinHeight = 60;
            DigitsAfterPoint = 3;
            ShowGraphInfo = false;
            IsFixedBound = false;
            UpdateBounds();
            UpdatePoints();
        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="name">You plot name</param>
        /// <param name="horizontalAxis">Horizontal axis name</param>
        /// <param name="verticalAxis">Vertical axis name</param>
        /// <param name="showGrid">If grid will be shown</param>
        /// <param name="gridColor">Grid color</param>
        /// <param name="gridMinWidth">Minimum width of the grid cell</param>
        /// <param name="gridMinHeight">Minimum height of the grid cell</param>
        /// <param name="digitsAfterPoint">Accuracy of displayed values</param>
        public SpotControl(
            string name,
            string horizontalAxis,
            string verticalAxis,
            bool showGrid,
            SolidColorBrush gridColor,
            int gridMinWidth = 60,
            int gridMinHeight = 60,
            int digitsAfterPoint = 3,
            bool showGraphInfo = false)
        {
            InitializeComponent();
            Graphs = new ObservableCollection<Graph>();
            ((INotifyCollectionChanged)lvInfo.Items).CollectionChanged += lvInfo_CollectionChanged;
            SpotName = name;
            HorizontalAxisName = horizontalAxis;
            VerticalAxisName = verticalAxis;
            ShowGrid = showGrid;
            GridColor = gridColor;
            MainBorder.Stroke = GridColor;
            GridMinWidth = gridMinWidth;
            GridMinHeight = gridMinHeight;
            DigitsAfterPoint = digitsAfterPoint;
            ShowGraphInfo = showGraphInfo;
            IsFixedBound = false;
            UpdateBounds();
            UpdatePoints();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds graph to 'Graphs' collection
        /// </summary>
        /// <param name="points">IEnumerable of points you want to add</param>
        /// <param name="color">Color of the graph</param>
        /// <param name="thickness">Graph thickness</param>
        /// <param name="name">Graph name</param>
        public void AddGraph(IEnumerable<Point> points, SolidColorBrush color, int thickness = 1, string name = "")
        {
            Polyline pl = new Polyline();
            foreach (var p in points)
            {
                pl.Points.Add(p);
            }
            pl.Stroke = color;
            pl.StrokeThickness = thickness;
            Graphs.Add(new Graph(name, pl));
            UpdateBounds();
            Update();
            if (ShowGraphInfo)
            {
                if (lvInfo.Visibility == Visibility.Hidden)
                {
                    lvInfo.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Removes graph from 'Graphs' collection by index
        /// </summary>
        /// <param name="index">Graph's index in 'Graphs' collection</param>
        public void RemoveGraph(int index)
        {
            try
            {
                Graphs.Remove(Graphs[index]);
                UpdateBounds();
                Update();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        /// <summary>
        /// Removes graph from 'Graphs' collection by name
        /// </summary>
        /// <param name="name">Graph's name</param>
        public void RemoveGraph(string name)
        {
            try
            {
                foreach (var g in Graphs)
                {
                    if (g.GraphName == name)
                    {
                        Graphs.Remove(g);
                        break;
                    }
                }
                UpdateBounds();
                Update();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        /// <summary>
        /// Adds point(s) to graph by index
        /// </summary>
        /// <param name="points">Point(s) you want to add</param>
        /// <param name="index">Graph's index in 'Graphs' collection</param>
        public void AddPointsToGraph(IEnumerable<Point> points, int index)
        {
            foreach (var p in points)
            {
                Graphs[index].GraphPolyline.Points.Add(p);
            }
            UpdateBounds();
            Update();
        }

        /// <summary>
        /// Adds point(s) to graph by name
        /// </summary>
        /// <param name="points">Point(s) you want to add</param>
        /// <param name="name">Graph's name</param>
        public void AddPointsToGraph(IEnumerable<Point> points, string name)
        {
            bool success = false;
            foreach (var g in Graphs)
            {
                if (g.GraphName == name)
                {
                    foreach (var p in points)
                    {
                        g.GraphPolyline.Points.Add(p);
                    }
                    success = true;
                    break;
                }
            }
            if (success)
            {
                UpdateBounds();
                Update();
            }
            else
            {
                throw new ArgumentException("There is no graph with the name '" + name + "'");
            }
        }

        /// <summary>
        /// Removes point(s) from graph by index
        /// </summary>
        /// <param name="points">Point(s) you want to remove</param>
        /// <param name="index">Graph's index in 'Graphs' collection</param>
        public void RemovePointsFromGraph(IEnumerable<Point> points, int index)
        {
            try
            {
                foreach (var p in Graphs[index].GraphPolyline.Points)
                {
                    if (points.Contains(p))
                    {
                        Graphs[index].GraphPolyline.Points.Remove(p);
                    }
                }
                UpdateBounds();
                Update();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        /// <summary>
        /// Removes point(s) from graph by name
        /// </summary>
        /// <param name="points">Point(s) you want to remove</param>
        /// <param name="name">Graph's name</param>
        public void RemovePointsFromGraph(IEnumerable<Point> points, string name)
        {
            bool success = false;
            foreach (var g in Graphs)
            {
                if (g.GraphName == name)
                {
                    foreach (var p in g.GraphPolyline.Points)
                    {
                        if (points.Contains(p))
                        {
                            g.GraphPolyline.Points.Remove(p);
                        }
                    }
                    success = true;
                }
                break;
            }
            if (success)
            {
                UpdateBounds();
                Update();
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Clears all graphs
        /// </summary>
        public void ClearGraphs()
        {
            Graphs.Clear();
            UpdateBounds();
            Update();
        }

        /// <summary>
        /// Updates canvas bounds
        /// </summary>
        public void UpdateBounds()
        {
            if (IsFixedBound)
            {
                Xmin = FixedBounds.Left;
                Ymin = FixedBounds.Top;
                Xmax = FixedBounds.Right;
                Ymax = FixedBounds.Bottom;
            }
            else
            {
                Xmin = 0;
                Ymin = 0;
                Xmax = 0;
                Ymax = 0;
                if (Graphs.Count > 0)
                {
                    if (Graphs[0].GraphPolyline.Points.Count > 0)
                    {
                        Xmin = Graphs[0].GraphPolyline.Points[0].X;
                        Ymin = Graphs[0].GraphPolyline.Points[0].Y;
                        Xmax = Graphs[0].GraphPolyline.Points[0].X;
                        Ymax = Graphs[0].GraphPolyline.Points[0].Y;
                    }
                }
                foreach (var graph in Graphs)
                {
                    var points = graph.GraphPolyline.Points.ToArray();
                    if (points.Length > 0)
                    {
                        foreach (var p in points)
                        {
                            if (p.X < Xmin) { Xmin = p.X; }
                            if (p.X > Xmax) { Xmax = p.X; }

                            if (p.Y < Ymin) { Ymin = p.Y; }
                            if (p.Y > Ymax) { Ymax = p.Y; }
                        }
                    }
                }
                bool isHorizontalLine = true;
                bool isVerticalLine = true;
                foreach (var graph in Graphs)
                {
                    var points = graph.GraphPolyline.Points.ToArray();
                    if (points.Length > 0)
                    {
                        foreach (var p in points)
                        {
                            if (p.X != Xmin) { isVerticalLine = false; }
                            if (p.X != Xmax) { isVerticalLine = false; }
                            if (p.Y != Ymin) { isHorizontalLine = false; }
                            if (p.Y != Ymax) { isHorizontalLine = false; }
                            if (!isHorizontalLine && !isVerticalLine) { break; }
                        }
                    }
                    if (!isHorizontalLine && !isVerticalLine) { break; }
                }
                if (isVerticalLine)
                {
                    Xmin -= 1;
                    Xmax += 1;
                }
                if (isHorizontalLine)
                {
                    Ymin -= 1;
                    Ymax += 1;
                }
            }
        }

        /// <summary>
        /// Updates points real coordinates (when scale changes)
        /// </summary>
        public void UpdatePoints()
        {
            MainCanvas.Children.Clear();
            if (CanvasWidth != 0 && CanvasHeight != 0)
            {
                foreach (var graph in Graphs)
                {
                    if (graph.GraphPolyline.Points.Count == 1)
                    {
                        Ellipse el = new Ellipse();
                        el.Width = graph.GraphPolyline.StrokeThickness;
                        el.Height = graph.GraphPolyline.StrokeThickness;
                        Point p = updateCoordinates(graph.GraphPolyline.Points[0]);
                        Canvas.SetLeft(el, p.X - el.Width / 2);
                        Canvas.SetTop(el, p.Y - el.Height / 2);

                        el.Fill = graph.GraphPolyline.Stroke;
                        MainCanvas.Children.Add(el);
                    }
                    else if (graph.GraphPolyline.Points.Count > 1)
                    {
                        Polyline pl = new Polyline();
                        foreach (var p in graph.GraphPolyline.Points)
                        {
                            pl.Points.Add(updateCoordinates(p));
                        }
                        pl.Stroke = graph.GraphPolyline.Stroke;
                        pl.StrokeThickness = graph.GraphPolyline.StrokeThickness;
                        MainCanvas.Children.Add(pl);
                    }
                }
            }
        }

        /// <summary>
        /// Updates grid and values drawing
        /// </summary>
        public void UpdateAxises()
        {
            GridAxises.Children.Clear();
            if (CanvasWidth != 0 && CanvasHeight != 0)
            {
                double margin = MainCanvas.Margin.Left;
                double sizeX = CanvasWidth;
                double sizeY = CanvasHeight;
                int nX = 1;
                int nY = 1;
                while (true)
                {
                    bool isX = false;
                    bool isY = false;
                    if (sizeX / 2 >= GridMinWidth) { sizeX /= 2; nX *= 2; }
                    else { isX = true; }
                    if (sizeY / 2 >= GridMinHeight) { sizeY /= 2; nY *= 2; }
                    else { isY = true; }
                    if (isX == true && isY == true) { break; }
                }
                if (ShowGrid)
                {
                    for (int i = 0; i < nX; ++i)
                    {
                        if (i != 0)
                        {
                            Line r1 = new Line();
                            r1.Fill = GridColor;
                            r1.Stroke = GridColor;
                            r1.StrokeThickness = 0.25;
                            r1.X1 = margin - 5;
                            r1.X2 = CanvasWidth + margin;
                            r1.Y1 = margin + i * sizeY;
                            r1.Y2 = r1.Y1;
                            GridAxises.Children.Add(r1);
                        }
                        for (int j = 0; j < nY; ++j)
                        {
                            if (j != 0)
                            {
                                Line r2 = new Line();
                                r2.Fill = GridColor;
                                r2.Stroke = GridColor;
                                r2.StrokeThickness = 0.25;
                                r2.X1 = margin + i * sizeX;
                                r2.X2 = r2.X1;
                                r2.Y1 = margin;
                                r2.Y2 = CanvasHeight + margin + 5;
                                GridAxises.Children.Add(r2);
                            }
                        }
                    }
                }

                for (int i = 0; i <= nX; ++i)
                {
                    TextBlock tX = new TextBlock();
                    tX.Text = Math.Round((Xmin + ((Xmax - Xmin) / nX) * i), DigitsAfterPoint).ToString();
                    tX.HorizontalAlignment = HorizontalAlignment.Left;
                    tX.VerticalAlignment = VerticalAlignment.Top;
                    tX.Width = sizeX;
                    tX.TextWrapping = TextWrapping.NoWrap;
                    tX.Margin = new Thickness(margin + i * sizeX + 3, margin + nY * sizeY, 0, 0);
                    GridAxises.Children.Add(tX);

                    for (int j = 0; j <= nY; ++j)
                    {
                        if (i == 0)
                        {
                            TextBlock tY = new TextBlock();
                            tY.Text = Math.Round((Ymax - j * (Ymax - Ymin) / nY), DigitsAfterPoint).ToString();
                            tY.HorizontalAlignment = HorizontalAlignment.Right;
                            tY.VerticalAlignment = VerticalAlignment.Top;
                            tY.Height = sizeY;
                            tY.TextWrapping = TextWrapping.NoWrap;
                            tY.Margin = new Thickness(0, j * sizeY + margin, margin + CanvasWidth + 3, 0);
                            GridAxises.Children.Add(tY);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates points and axises with grid
        /// </summary>
        public void Update()
        {
            CanvasWidth = MainCanvas.ActualWidth;
            CanvasHeight = MainCanvas.ActualHeight;
            UpdatePoints();
            UpdateAxises();
        }

        /// <summary>
        /// Converts real points coordinates to points coordinates in canvas
        /// </summary>
        /// <param name="p">Point you want to convert</param>
        /// <returns>Converted point</returns>
        protected Point updateCoordinates(Point p)
        {
            Point res = new Point();
            res.X = ((p.X - Xmin) / (Xmax - Xmin)) * CanvasWidth;
            res.Y = ((p.Y - Ymin) / (Ymax - Ymin)) * CanvasHeight;
            res = updateY(res);
            return res;
        }

        /// <summary>
        /// Inverts ordinate
        /// </summary>
        /// <param name="p">Point in which you want to invert p.Y</param>
        /// <returns>Inverted p.Y</returns>
        protected Point updateY(Point p)
        {
            Point res = new Point();
            res.X = p.X;
            res.Y = CanvasHeight - p.Y;
            return res;
        }

        /// <summary>
        /// Fixes bounds of graph axises
        /// </summary>
        /// <param name="left">Left bound</param>
        /// <param name="top">Top bound</param>
        /// <param name="right">Right bound</param>
        /// <param name="bottom">Bottom bound</param>
        public void FixBounds(double left, double top, double right, double bottom)
        {
            IsFixedBound = true;
            FixedBounds = new Rect(Math.Min(left, right), Math.Min(top, bottom), Math.Abs(right - left), Math.Abs(bottom - top));
            UpdateBounds();
            Update();
        }

        /// <summary>
        /// Unfixes bounds of graph axises
        /// </summary>
        public void UnfixBounds() { IsFixedBound = false; }

        /// <summary>
        /// Saves graph as images
        /// </summary>
        /// <param name="filename">File name with absolute path</param>
        /// <param name="format">File format</param>
        public void SaveAsImage(string filename, PlotImageFormat format)
        {
            BitmapEncoder encoder = null;
            switch (format)
            {
                case PlotImageFormat.bmp:
                    encoder = new BmpBitmapEncoder();
                    break;
                case PlotImageFormat.png:
                    encoder = new PngBitmapEncoder();
                    break;
                case PlotImageFormat.jpg:
                    encoder = new JpegBitmapEncoder();
                    break;
                default:
                    encoder = new PngBitmapEncoder();
                    break;
            }

            saveUsingEncoder(filename, encoder);
        }

        private void saveUsingEncoder(string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                   (int)this.ActualWidth,
                   (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visualToRender);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        #endregion

        #region Events

        private void SpotControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }

        private void lvInfo_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GridView gv = lvInfo.View as GridView;
            if (gv != null)
            {
                foreach (GridViewColumn gvc in gv.Columns)
                {
                    gvc.Width = gvc.ActualWidth;
                    gvc.Width = Double.NaN;
                }
            }
        }

        #endregion
    }

    public enum PlotImageFormat { png, jpg, bmp };
}
