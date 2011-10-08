using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Framer.Model
{
    public class WorldModel: INotifyPropertyChanged {
        public IList<ImageInfoModel> Images { get; set; }
        public IList<FrameInfoModel> Frames { get; set; }

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

        public WorldModel(string imagesDir, string framesDir) {
            PrintPreviewZoom = 1;
            PageWidth = 1024;
            ImagesPerRow = 3;

            ThumbnailSize = 200;
            Images = new[] {"*.png", "*.jpg", "*.bmp", "*.gif"}
                .SelectMany(pattern => Directory.GetFiles(imagesDir, pattern))
                .Select(fn => new ImageInfoModel {Path = fn})
                .ToList();

            Frames = new[] {"*.png", "*.gif"}
                .SelectMany(pattern => Directory.GetFiles(framesDir, pattern))
                .Select(fn => new FrameInfoModel {Path = fn, WorldModel = this})
                .ToList();

            FlattenedImageList = new ObservableCollection<PrintedImageModel>();
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
        }

        private void RebuildFlattenedImageList() {
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
        public string Path { get; set; }
        private double m_brightness;

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

        public ImageInfoModel() {
            ImagesCount = 1;
            Brightness = 0;
            Contrast = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
