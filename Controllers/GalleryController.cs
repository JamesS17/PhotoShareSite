using ImageGallery.Data;
using Microsoft.AspNetCore.Mvc;
using PhotoShareSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhotoShareSite.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IImage _imageService;

        public GalleryController(IImage imageService)
        {
            _imageService = imageService;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var imageList = _imageService.GetGallery(userId);
           // var imageList2 = _imageService.GetImgOther(userId);
            var model = new GalleryIndexModel()
            {
                Images = imageList,
               // OtherImages= imageList2,
                SearchQuery = ""

            };







            return View(model);
        }

        public IActionResult Detail(int id)
        {

            var image = _imageService.GetById(id);
            var model = new GalleryDetailModel()
            {
                Id = image.Id,
                Title = image.Title,
                CreatedOn = image.CreatedOn,
                Url = image.Url,
                Tags = image.Tags.Select(t => t.Description).ToList(),
                UserName = image.UserName,
                GeoLocation = image.GeoLocation
            };

            return View(model);
        }
    }
}
