using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Framer.Model;
using Microsoft.Win32;
using Button = System.Windows.Controls.Button;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace Framer
{
    /// <summary>
    /// Interaction logic for DesignControl.xaml
    /// </summary>
    public partial class DesignControl : UserControl
    {
        public DesignControl()
        {
            InitializeComponent();
        }

        private void OnImagesListDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void OnImageCountDecrease(object sender, RoutedEventArgs e)
        {
            var mdl = (ImageInfoModel)((Button)sender).DataContext;
            if (mdl.ImagesCount > 0)
                mdl.ImagesCount--;
        }

        private void OnImageCountIncrease(object sender, RoutedEventArgs e)
        {
            var mdl = (ImageInfoModel)((Button)sender).DataContext;
            mdl.ImagesCount++;
        }

        private void Frame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var mdl = (FrameInfoModel)((Image)sender).DataContext;
            mdl.WorldModel.SelectedFrame = mdl;
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var world = (WorldModel)DataContext;
            var imgInfo = (ImageInfoModel)((FramedImage)sender).DataContext;
            if (world.SelectedFrame != null)
                imgInfo.Frame = world.SelectedFrame;
        }

        private void ChooseFramesDir_Click(object sender, RoutedEventArgs e) {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.Cancel) return;

            App.Settings.FramesDirectory = dlg.SelectedPath;
            App.Settings.Save();

            var world = (WorldModel) DataContext;
            world.FramesDirectory = dlg.SelectedPath;
        }

        private void ChooseImagesDir_Click(object sender, RoutedEventArgs e) {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.Cancel) return;

            App.Settings.ImagesDirectory = dlg.SelectedPath;
            App.Settings.Save();

            var world = (WorldModel)DataContext;
            world.ImagesDirectory = dlg.SelectedPath;            
        }
    }
}
