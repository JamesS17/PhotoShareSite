using Microsoft.AspNetCore.Http;

namespace PhotoShareSite.Models
{
    public class UploadImageModel
    {
        public string Title { get; set; }
        public string Tags { get; set; }
        public IFormFile ImageUpload { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string GeoLocation { get; set; }
    }
}
