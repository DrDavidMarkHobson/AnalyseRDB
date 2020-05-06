using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
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
using SystemInterface.IO;
using SystemWrapper.IO;
using Microsoft.Win32;
using RDB.Interface.RDBObjects;
using RDBData.Points;
using RDBData.Reader;
using Point = System.Windows.Point;

namespace RDBForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<UIElement> RDBNetPoints;

        private RdbNets _loadedData { get; set; }

        private Point RelativePoint(Pin pin)
        {
            return new Point {X = pin.x, Y = pin.y};
        }

        private static Line RDBPin(Point pin)
        {
            Line myLine = new Line();
            myLine.Stroke = Brushes.LightSteelBlue;
            myLine.X1 = pin.X;
            myLine.X2 = pin.X;
            myLine.Y1 = pin.Y;
            myLine.Y2 = pin.Y;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;

            return myLine;
        }

        public static ImageSource ToImageSource(System.Drawing.Image image, ImageFormat imageFormat)
        {
            BitmapImage bitmap = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                // Save to the stream
                image.Save(stream, imageFormat);

                // Rewind the stream
                stream.Seek(0, SeekOrigin.Begin);

                // Tell the WPF BitmapImage to use this stream
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }

            return bitmap;
        }

        private void LoadData()
        {
            RDBNetPoints = new List<UIElement>();
            var readImage = new RenderRdb();

            if (_loadedData.fileName != null && _loadedData.fileName != "")
            {
                var newImage = readImage.Convert(_loadedData);

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = ToImageSource(newImage, ImageFormat.Bmp);
                CanvasImage.Source = brush.ImageSource;
                RDBCanvas.Children.Clear();
                RDBCanvas.Children.Add(CanvasImage);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _loadedData = new RdbNets { fileName = "", Nets = new RdbNet[]{}};
            RDBNetPoints = new List<UIElement>();

            Line myLine = new Line();
            myLine.Stroke = Brushes.LightSteelBlue;
            myLine.X1 = 1;
            myLine.X2 = 50;
            myLine.Y1 = 1;
            myLine.Y2 = 50;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            RDBNetPoints.Add(myLine);

            LoadData();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;
            }

            GetRdbNets getter = new GetRdbNets(new ReadRdb(new OpenAndReadRDBFile()));
            _loadedData = getter.Get(fileName);
            //MessageBox.Show(fileName);
            LoadData();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("text", "caption");
        }

        private void Rotate_Click(object sender, RoutedEventArgs e)
        {
            UpdateRDB rotator = new UpdateRDB();

            _loadedData = rotator.RotateAround(
                _loadedData,
                new RDB.Interface.RDBObjects.Point
                {
                    X = _loadedData.CentroidX, 
                    Y = _loadedData.CentroidY
                },
                (float)45
            );

            LoadData();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
