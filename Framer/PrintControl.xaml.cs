using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
                var grdPage = brdPreview;
                grdPage.Height = dlg.PrintableAreaHeight + 50;
                grdPage.Width = dlg.PrintableAreaWidth;
                //grdPage.Width = dlg.PrintableAreaWidth;
                grdPage.Measure(new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));


                var printArea = dlg.PrintQueue.GetPrintCapabilities().PageImageableArea;
                var origin = printArea != null
                                 ? new Point(printArea.OriginWidth, printArea.OriginHeight)
                                 : new Point(0, 0);
                var extent = printArea != null
                                 ? new Point(printArea.ExtentWidth, printArea.ExtentHeight)
                                 : new Point(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);

                grdPage.Arrange(new Rect(origin, extent));

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
    }
}
