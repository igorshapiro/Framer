using System;
using System.IO;
using System.Linq;
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
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            DataContext = GetImagesList();
        }

        public WorldModel GetImagesList() {
            var cmdLineArgs = Environment.GetCommandLineArgs();
            string imagesFolder;
            if (cmdLineArgs.Length > 1)
                imagesFolder = cmdLineArgs[1];
            else {
                imagesFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
                if (!Directory.Exists(imagesFolder))
                    imagesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                else if (!Directory.Exists(imagesFolder))
                    imagesFolder = null;
            }
            return new WorldModel(imagesFolder);
        }

        private void OnFileExitClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void OnFileOptionsClick(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e) {
            var world = (WorldModel) DataContext;
            foreach (var img in world.Images) {
                img.IsSelected = true;
            }
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e) {
            var world = (WorldModel) DataContext;
            foreach (var img in world.Images) {
                img.IsSelected = false;
            }
        }

        private void ApplyToSelected_Click(object sender, RoutedEventArgs e) {
            var world = (WorldModel) DataContext;
            if (world.SelectedFrame == null) return;

            foreach (var img in world.Images.Where(img => img.IsSelected)) {
                img.Frame = world.SelectedFrame;
            }
        }
    }
}
