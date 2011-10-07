using System;
using System.Collections.Generic;
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
            icList.DataContext = GetImagesList();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            DataContext = GetImagesList();
        }

        public ImagesListModel GetImagesList() {
            return new ImagesListModel
                   {
                       Images = new List<ImageInfoModel>
                                {
                                    new ImageInfoModel
                                    {
                                        Path = @"C:\Users\Public\Pictures\Sample Pictures\Chrysanthemum.jpg"
                                    }
                                }
                   };
        }

        private void OnFileExitClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void OnFileOptionsClick(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void OnImagesListDragEnter(object sender, DragEventArgs e) {
            e.Effects = DragDropEffects.Copy;
        }
    }
}
