using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoShareSite.Models
{
    public class EditPhotoModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public string GeoLocation { get; set; }
    }
}
