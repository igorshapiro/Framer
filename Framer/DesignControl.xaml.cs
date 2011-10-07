using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Framer.Model;

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
            var imgInfo = (ImageInfoModel)((Image)sender).DataContext;
            if (world.SelectedFrame != null)
                imgInfo.Frame = world.SelectedFrame;
        }
    }
}
