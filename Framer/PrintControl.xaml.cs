using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Framer.Model;

namespace Framer {
    public partial class PrintControl: INotifyPropertyChanged {
        private PrintDialog m_printDialog;

        private double m_pageHeight;
        public double PageHeight {
            get { return m_pageHeight; }
            set {
                m_pageHeight = value;
                OnPropertyChanged("PageHeight");
            }
        }

        private double m_pageWidth;
        public double PageWidth {
            get { return m_pageWidth; }
            set {
                m_pageWidth = value;
                OnPropertyChanged("PageWidth");
            }
        }

        private double m_originX;
        public double OriginX {
            get { return m_originX; }
            set {
                m_originX = value;
                OnPropertyChanged("OriginX");
            }
        }

        private double m_originY;
        public double OriginY {
            get { return m_originY; }
            set {
                m_originY = value;
                OnPropertyChanged("OriginY");
            }
        }

        public PrintControl() {
            InitializeComponent();
        }

        private void SelectPrinter_Click(object sender, RoutedEventArgs e) {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog() == true) {
                PageHeight = dlg.PrintableAreaHeight + 50;
                PageWidth = dlg.PrintableAreaWidth;

                var printArea = dlg.PrintQueue.GetPrintCapabilities().PageImageableArea;
                if (printArea != null) {
                    OriginX = printArea.OriginWidth;
                    OriginY = printArea.OriginHeight;
                }
                else {
                    OriginX = OriginY = 0;
                }

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
            
            var png = new PngBitmapEncoder();
            foreach (var pageElement in GetPageElements()) {
                var bmp = new RenderTargetBitmap((int)PageWidth, (int)PageHeight, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(pageElement);
                png.Frames.Add(BitmapFrame.Create(bmp));
            }            
            using (var stream = File.Create(string.Format("{0:yyyyMMddhhmmss}.png", DateTime.Now))) {
                png.Save(stream);
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e) {
            if (m_printDialog == null) {
                MessageBox.Show("Please select a printer first");
                return;
            }

            m_printDialog.PrintDocument(new Paginator(GetPageElements(), new Size(PageWidth, PageHeight)), null);
        }

        private Visual[] GetPageElements() {
            var grids = new List<Visual>();
            foreach (var item in icPages.Items)
            {
                var container = (ContentPresenter)icPages.ItemContainerGenerator.ContainerFromItem(item);
                container.ApplyTemplate();
                var grd = (Grid) container.ContentTemplate.FindName("grdPage", container);
                grids.Add(grd);
            }
            return grids.ToArray();
        }
    }

    public class Paginator: DocumentPaginator {
        private readonly Visual[] m_visuals;
        private readonly Size m_size;

        public Paginator(Visual[] visuals, Size size) {
            m_visuals = visuals;
            m_size = size;
        }

        public override DocumentPage GetPage(int pageNumber) {
            return new DocumentPage(m_visuals[pageNumber]);
        }

        public override bool IsPageCountValid {
            get { return true; }
        }

        public override int PageCount {
            get { return m_visuals.Length; }
        }

        public override Size PageSize {
            get { return m_size; }
            set { throw new NotImplementedException(); }
        }

        public override IDocumentPaginatorSource Source {
            get { return null; }
        }
    }
}
