using System.Collections.Generic;

namespace Framer.Model
{
    public class ImagesListModel {
        public IList<ImageInfoModel> Images { get; set; }
        public double Width { get; set; }

        public ImagesListModel() {
            Width = 200;
        }
    }

    public class ImageInfoModel {
        public string Path { get; set; }
    }
}
