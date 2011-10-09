using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Framer.Model
{
    public class WorldModel: INotifyPropertyChanged {
        private IList<ImageInfoModel> m_images;
        public IList<ImageInfoModel> Images {
            get { return m_images; }
            set {
                m_images = value;
                RebuildPages();
                OnPropertyChanged("Images");
            }
        }

        private IList<FrameInfoModel> m_frames;
        public IList<FrameInfoModel> Frames {
            get { return m_frames; }
            set {
                m_frames = value;
                OnPropertyChanged("Frames");
            }
        }

        private int m_pageWidth;
        public int PageWidth {
            get { return m_pageWidth; }
            set {
                m_pageWidth = value;

                if (ImagesPerRow != 0)
                    PrintedImageSize = value/ImagesPerRow;

                RebuildPages();

                OnPropertyChanged("PageWidth");
            }
        }

        private int m_pageHeight;
        public int PageHeight {
            get { return m_pageHeight; }
            set {
                m_pageHeight = value;

                RebuildPages();

                OnPropertyChanged("PageHeight");
            }
        }

        private int m_imagesPerRow;
        public int ImagesPerRow {
            get { return m_imagesPerRow; }
            set {
                m_imagesPerRow = value;

                if (value != 0)
                    PrintedImageSize = PageWidth / value;

                RebuildPages();
                OnPropertyChanged("ImagesPerRow");
            }
        }

        private int m_printedImageSize;
        public int PrintedImageSize {
            get { return m_printedImageSize; }
            set {
                m_printedImageSize = value;
                OnPropertyChanged("PrintedImageSize");
            }
        }

        private double m_thumbnailSize;
        public double ThumbnailSize {
            get { return m_thumbnailSize; }
            set {
                m_thumbnailSize = value;
                OnPropertyChanged("ThumbnailSize");
                
            }
        }

        private IList<Page> m_pages;
        public IList<Page> Pages {
            get { return m_pages; }
            set {
                m_pages = value;
                OnPropertyChanged("Pages");
            }
        }

        private FrameInfoModel m_selectedFrame;
        public FrameInfoModel SelectedFrame {
            get { return m_selectedFrame; }
            set {
                if (value != null)
                    m_selectedFrame = value;
                foreach (var frame in Frames) {
                    frame.IsSelected = ReferenceEquals(value, frame);
                }
            }
        }

        public double PrintPreviewZoom { get; set; }

        private string m_framesDirectory;
        public string FramesDirectory {
            get { return m_framesDirectory; }
            set {
                if (string.IsNullOrEmpty(value) || !Directory.Exists(value)) return;
                m_framesDirectory = value;
                Frames = new[] { "*.png", "*.gif" }
                    .SelectMany(pattern => Directory.GetFiles(value, pattern))
                    .Select(fn => new FrameInfoModel { Path = fn, WorldModel = this })
                    .ToList();

                OnPropertyChanged("FramesDirectory");
            }
        }

        private string m_imagesDirectory;
        public string ImagesDirectory {
            get { return m_imagesDirectory; }
            set {
                if (string.IsNullOrEmpty(value) || !Directory.Exists(value)) return;
                m_imagesDirectory = value;
                var list = new List<ImageInfoModel>();
                foreach (var pattern in new[] {"*.png", "*.jpg", "*.bmp", "*.gif"})
                    foreach (var file in Directory.GetFiles(value, pattern)) {
                        try {
                            list.Add(new ImageInfoModel(file));
                        }
                        catch(Exception) {
                            MessageBox.Show(@"Unable to load image " + file);
                        }
                    }
                        
                Images = list;
                OnPropertyChanged("ImagesDirectory");
            }
        }

        public bool HasImagesSelected {
            get { return Images.Any(img => img.IsSelected); }
            set {}
        }

        public WorldModel(string imagesDir) {
            PrintPreviewZoom = 1;
            PageWidth = 860;
            PageHeight = 1056;
            ImagesPerRow = 3;

            ThumbnailSize = 200;

            FramesDirectory = App.Settings.FramesDirectory;
            ImagesDirectory = imagesDir;

            RebuildPages();

            foreach (var img in Images) {
                img.PropertyChanged += ImagePropertyChangedHandler;
            }
        }

        private void ImagePropertyChangedHandler(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "ImagesCount") {
                RebuildPages();
                OnPropertyChanged("FlattenedImageList");
            }
            else if (e.PropertyName == "IsSelected") {
                OnPropertyChanged("HasImagesSelected");
            }
        }

        private void RebuildPages() {
            if (Images == null) return;
            var flatList = new List<ImageInfoModel>();
            foreach (var img in Images)
            {
                for (int i = 0; i < img.ImagesCount; i++)
                {
                    flatList.Add(img);
                }
            }

            var pages = new List<Page>();
            int imageIndex = 0;
            while (imageIndex < flatList.Count) {
            // Yes yes yes! Goto pending... 
            nextPage:
                var page = new Page {PageString = (pages.Count + 1).ToString()};
                pages.Add(page);
                int totalHeight = 0;
                int imageOnPageIndex = 0;

                while (totalHeight <= PageHeight && imageIndex < flatList.Count) {
                    int maxHeight = 0;
                    for (int column = 0; column < ImagesPerRow && imageIndex < flatList.Count; column++) {
                        var img = flatList[imageIndex];
                        var height = (int) (img.Source.Height * (PageWidth / (double)ImagesPerRow) / img.Source.Width);
                        if (height > maxHeight) maxHeight = height;

                        if (totalHeight + height > PageHeight) goto nextPage;

                        page.Images.Add(new PrintedImageModel
                                        {
                                            Image = img, 
                                            Column = imageOnPageIndex % ImagesPerRow, 
                                            Row = imageOnPageIndex / ImagesPerRow
                                        });
                        imageIndex++;
                        imageOnPageIndex++;
                    }
                    totalHeight += maxHeight;
                }
            }
            Pages = pages;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Page {
        public IList<PrintedImageModel> Images { get; set; }
        public string PageString { get; set; }

        public Page() {
            Images = new List<PrintedImageModel>();
        }
    }

    public class PrintedImageModel {
        public ImageInfoModel Image { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class FrameInfoModel: INotifyPropertyChanged {
        public WorldModel WorldModel { get; set; }

        public string Path { get; set; }
        private bool m_isSelected;
        public bool IsSelected {
            get { return m_isSelected; }
            set {
                m_isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ImageInfoModel: INotifyPropertyChanged {
//        public string Path { get; set; }
        public ImageSource Source { get; set; }
        private double m_brightness;

        private bool m_isSelected;
        public bool IsSelected {
            get { return m_isSelected; }
            set {
                m_isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        private FrameInfoModel m_frame;
        public FrameInfoModel Frame {
            get { return m_frame; }
            set {
                m_frame = value;
                OnPropertyChanged("Frame");
            }
        }

        public double Brightness {
            get { return m_brightness; }
            set {
                m_brightness = value;
                OnPropertyChanged("Brightness");
            }
        }

        private double m_contrast;
        public double Contrast {
            get { return m_contrast; }
            set {
                m_contrast = value;
                OnPropertyChanged("Contrast");
            }
        }

        private int m_imagesCount;
        public int ImagesCount {
            get { return m_imagesCount; }
            set {
                m_imagesCount = value;
                OnPropertyChanged("ImagesCount");
            }
        }

        public ImageInfoModel(string path) {
            ImagesCount = 1;
            Brightness = 0;
            Contrast = 0;
            IsSelected = true;

            BitmapDecoder decoder = BitmapDecoder.Create(new Uri(path), BitmapCreateOptions.None, BitmapCacheOption.None);
            BitmapFrame frame = decoder.Frames[0];
            var rotation = frame.PixelHeight > frame.PixelWidth ? Rotation.Rotate90 : Rotation.Rotate0;

            byte[] buffer = File.ReadAllBytes(path);
            var ms = new MemoryStream(buffer);

            var bmp = new BitmapImage();
            bmp.BeginInit();

            bmp.DecodePixelWidth = 1200;
            bmp.StreamSource = ms;

            bmp.Rotation = rotation;

            bmp.EndInit();
            bmp.Freeze();

            Source = bmp;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
