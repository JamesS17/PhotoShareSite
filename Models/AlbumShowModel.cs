using ImageGallery.Data.Models;
using System.Collections.Generic;

namespace PhotoShareSite.Models
{
    public class AlbumShowModel
    {
        public IEnumerable<GalleryImage> Images { get; set; }
        public IEnumerable<PAlbum> Albums { get; set; }
        public int albumQuery { get; set; }
       // public List<string> Albums { get; set; }
    }
}
