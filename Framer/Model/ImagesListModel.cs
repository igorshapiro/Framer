using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Framer.Model
{
    public class ImagesListModel {
        public IList<ImageInfoModel> Images { get; set; }
        public double ThumbnailSize { get; set; }

        public ImagesListModel(string imagesDir) {
            ThumbnailSize = 200;
            Images = 
                new[]{"*.png", "*.jpg", "*.bmp", "*.gif"}
                .SelectMany(pattern => Directory.GetFiles(imagesDir, pattern))
                .Select(fn => new ImageInfoModel {Path = fn})
                .ToList();
        }
    }

    public class ImageInfoModel: INotifyPropertyChanged {
        public string Path { get; set; }

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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
