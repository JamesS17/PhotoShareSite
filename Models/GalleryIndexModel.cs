using ImageGallery.Data.Models;
using System.Collections.Generic;

namespace PhotoShareSite.Models
{
    public class GalleryIndexModel
    {
        public IEnumerable<GalleryImage> Images { get; set; }
        public IEnumerable<GalleryImage> OtherImages { get; set; }
        public string SearchQuery { get; set; }
    }
}
