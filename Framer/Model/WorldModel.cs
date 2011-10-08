using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                RebuildFlattenedImageList();
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

                OnPropertyChanged("PageWidth");
            }
        }

        private int m_imagesPerRow;
        public int ImagesPerRow {
            get { return m_imagesPerRow; }
            set {
                m_imagesPerRow = value;

                if (value != 0)
                    PrintedImageSize = PageWidth / value;

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

        public ObservableCollection<PrintedImageModel> FlattenedImageList { get; set; }

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
                foreach (string pattern in new[] {"*.png", "*.jpg", "*.bmp", "*.gif"})
                    foreach (string file in Directory.GetFiles(value, pattern)) {
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
            set {
                
            }
        }

        public WorldModel(string imagesDir) {
            PrintPreviewZoom = 1;
            PageWidth = 1056;
            ImagesPerRow = 3;

            ThumbnailSize = 200;

            FlattenedImageList = new ObservableCollection<PrintedImageModel>();

            FramesDirectory = App.Settings.FramesDirectory;
            ImagesDirectory = imagesDir;

            RebuildFlattenedImageList();

            foreach (var img in Images) {
                img.PropertyChanged += ImagePropertyChangedHandler;
            }
        }

        private void ImagePropertyChangedHandler(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "ImagesCount") {
                RebuildFlattenedImageList();
                OnPropertyChanged("FlattenedImageList");
            }
            else if (e.PropertyName == "IsSelected") {
                OnPropertyChanged("HasImagesSelected");
            }
        }

        private void RebuildFlattenedImageList() {
            if (Images == null) return;
            FlattenedImageList.Clear();
            int imageIndex = 0;
            foreach (var img in Images)
            {
                for (int i = 0; i < img.ImagesCount; i++)
                {
                    FlattenedImageList.Add(new PrintedImageModel {Image = img, Column = imageIndex / ImagesPerRow, Row = imageIndex % ImagesPerRow});
                    imageIndex++;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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

            byte[] buffer = File.ReadAllBytes(path);
            var ms = new MemoryStream(buffer);

            var bmp = new BitmapImage();
            bmp.BeginInit();

            bmp.DecodePixelWidth = 500;

            bmp.StreamSource = ms;

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
