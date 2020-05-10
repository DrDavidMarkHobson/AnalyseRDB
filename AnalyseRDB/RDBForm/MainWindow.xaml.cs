using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using RDB.Interface.RDBObjects;
using RDBData.Points;
using RDBData.Reader;
using RDBData.Render;
using RDBData.Writer;

namespace RDBForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RdbNets _loadedData;
        private UpdateRDB _updateRDB;
        private int xPosition;
        private int yPosition;
        private float _angle;

        private BackgroundWorker bgw;

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
            bgw = new BackgroundWorker();
            _updateRDB = new UpdateRDB();
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
            RotateAngle.IsEnabled = false;
            bgw = new BackgroundWorker();
            ProgressBar.Value = 0;
            _angle = Convert.ToSingle(RDBAngle.Text);

            bgw.DoWork += new DoWorkEventHandler(bgw_Rotate);
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.WorkerReportsProgress = true;
            bgw.RunWorkerAsync();
        }

        async void bgw_Rotate(object sender, DoWorkEventArgs e)
        {
            _updateRDB.BackgroundWorker = bgw;

            _loadedData = await _updateRDB.RotateAround(
                _loadedData,
                new RDB.Interface.RDBObjects.Point
                {
                    X = xPosition,
                    Y = yPosition
                },
                _angle
            );
        }

        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
            if (e.UserState != null && (int)e.UserState > 1)
            {
                ProgressBar.Value = ProgressBar.Value + 50;
            }
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBar.Value = 0;
            Thread.Sleep(1);
            bgw = null;
            LoadData();
            RotateAngle.IsEnabled = true;
        }

        private void CanvasImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            xPosition = (int)Mouse.GetPosition(Application.Current.MainWindow).X;
            yPosition = (int)Mouse.GetPosition(Application.Current.MainWindow).Y-44;
            XPosition.Content = xPosition;
            YPosition.Content = yPosition;
        }

        private void DoThread_Click(object sender, RoutedEventArgs e)
        {
            /*
            ProgressBar.Value = 0;

            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.WorkerReportsProgress = true;
            bgw.RunWorkerAsync();
            */
        }
    }
}
