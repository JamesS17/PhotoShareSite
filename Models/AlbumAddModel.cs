using ImageGallery.Data.Models;
using System.Collections.Generic;

namespace PhotoShareSite.Models
{
    public class AlbumAddModel
    {
        public IEnumerable<PAlbum> Albums { get; set; }
        public string AlbumName { get; set; }
        public int albumQuery { get; set; }
        public int PhotoId { get; set; }
    }
}
