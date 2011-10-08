using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Framer.Model;

namespace Framer {
    public partial class PrintControl: INotifyPropertyChanged {
        private PrintDialog m_printDialog;

        public PrintControl() {
            InitializeComponent();
        }

        private void SelectPrinter_Click(object sender, RoutedEventArgs e) {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog() == true) {
                brdPreview.Height = dlg.PrintableAreaHeight + 50;
                brdPreview.Width = dlg.PrintableAreaWidth;
                brdPreview.Measure(new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));


                var printArea = dlg.PrintQueue.GetPrintCapabilities().PageImageableArea;
                var origin = printArea != null
                                 ? new Point(printArea.OriginWidth, printArea.OriginHeight)
                                 : new Point(0, 0);
                var extent = printArea != null
                                 ? new Point(printArea.ExtentWidth, printArea.ExtentHeight)
                                 : new Point(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);

                brdPreview.Arrange(new Rect(origin, extent));

                m_printDialog = dlg;

                var world = (WorldModel) DataContext;
                world.PageWidth = (int) (printArea == null ? dlg.PrintableAreaWidth : printArea.ExtentWidth);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SaveFiles_Click(object sender, RoutedEventArgs e) {
            var bmp = new RenderTargetBitmap((int) brdPreview.ActualWidth, (int) brdPreview.ActualHeight, 120, 96, PixelFormats.Pbgra32);
            bmp.Render(grdPage);
            
            var png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));
            using (var stream = File.Create(string.Format("{0:yyyyMMddhhmmss}.png", DateTime.Now))) {
                png.Save(stream);
            }
        }
    }
}
