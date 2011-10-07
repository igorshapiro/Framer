using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
            return new ImagesListModel(@"C:\Users\Public\Pictures\Sample Pictures");
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

        private void OnImageCountDecrease(object sender, RoutedEventArgs e) {
            var mdl = (ImageInfoModel) ((Button) sender).DataContext;
            if (mdl.ImagesCount > 0)
                mdl.ImagesCount--;
        }

        private void OnImageCountIncrease(object sender, RoutedEventArgs e) {
            var mdl = (ImageInfoModel)((Button)sender).DataContext;
            mdl.ImagesCount++;
        }
    }
}
