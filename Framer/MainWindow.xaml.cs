using System;
using System.Windows;
using Framer.Model;

namespace Framer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
//            ctrlDesign.DataContext = GetImagesList();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            DataContext = GetImagesList();
        }

        public WorldModel GetImagesList() {
            return new WorldModel(@"C:\Users\Public\Pictures\Sample Pictures",
                @"C:\Users\Public\Pictures\Frames");
        }

        private void OnFileExitClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void OnFileOptionsClick(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }
    }
}
