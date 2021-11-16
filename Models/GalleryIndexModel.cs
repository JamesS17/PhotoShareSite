using ImageGallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoShareSite.Models
{
    public class GalleryIndexModel
    {
        public IEnumerable<GalleryImage> Images { get; set; }
        //public IEnumerable<GalleryImage> OtherImages { get; set; }
        public string SearchQuery { get; set; }
    }
}
