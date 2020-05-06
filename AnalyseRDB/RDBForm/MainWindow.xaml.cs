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
using RDBData.Render;
using RDBData.Writer;
using Point = System.Windows.Point;

namespace RDBForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RdbNets _loadedData;
        private int xPosition;
        private int yPosition;

        public static ImageSource ToImageSource(
            System.Drawing.Image image, 
            ImageFormat imageFormat)
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
            var readImage = new RenderRdb();
            cenXPosition.Content = _loadedData.CentroidX;
            cenYPosition.Content = _loadedData.CentroidY;

            if (!string.IsNullOrEmpty(_loadedData.fileName))
            {
                var newImage = readImage.Convert(_loadedData);

                ImageBrush brush = new ImageBrush
                {
                    ImageSource = ToImageSource(newImage, ImageFormat.Bmp)
                };
                CanvasImage.Source = brush.ImageSource;
                RDBCanvas.Children.Clear();
                RDBCanvas.Children.Add(CanvasImage);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _loadedData = new RdbNets { fileName = "", Nets = new RdbNet[]{}};
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
            LoadData();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveRdbNetsToFile save = new SaveRdbNetsToFile();
                _loadedData.fileName = saveFileDialog.FileName;
                save.Write(_loadedData);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void RotateAngle_Click(object sender, RoutedEventArgs e)
        {
            UpdateRDB rotator = new UpdateRDB();
            var angle = Convert.ToSingle(RDBAngle.Text);

            _loadedData = rotator.RotateAround(
                _loadedData,
                new RDB.Interface.RDBObjects.Point
                {
                    X = xPosition,
                    Y = yPosition
                },
                angle
            );

            LoadData();
        }

        private void CanvasImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            xPosition = (int)Mouse.GetPosition(Application.Current.MainWindow).X;
            yPosition = (int)Mouse.GetPosition(Application.Current.MainWindow).Y-44;
            XPosition.Content = xPosition;
            YPosition.Content = yPosition;
        }
    }
}
