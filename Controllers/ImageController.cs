using ImageGallery.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PhotoShareSite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhotoShareSite.Controllers
{
    public class ImageController : Controller
    {
        private IConfiguration _config;
        private IImage _imageService;
        private string AzureConnectionString { get; }


        public ImageController(IConfiguration config, IImage imageService)
        {
            _config = config;
            _imageService = imageService;
            AzureConnectionString = _config["AzureStorageConnectionString"];

        }
        [Authorize]
        public IActionResult Upload()
        {
            var model = new UploadImageModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadNewImage(IFormFile file, string title, string tags, string geoLocation)
        {
            

                var container = _imageService.GetBlobContainer(AzureConnectionString, "photodb");
            var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = content.FileName.Trim('"');
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
                var userName = User.FindFirstValue(ClaimTypes.Name);

                var blob = container.GetBlockBlobReference(fileName);
            await blob.UploadFromStreamAsync(file.OpenReadStream());
            await _imageService.SetImage(title, tags, blob.Uri, userId, userName, geoLocation);



            return RedirectToAction("Index", "Gallery");
        }

        public IActionResult EditPhoto(int id)
        {

            var image = _imageService.GetById(id);
            string imgT = "*$%";
            foreach (var item in image.Tags.Select(t => t.Description).ToList())
            {
                imgT = imgT+", " + item;
            }
            imgT = imgT.Replace("*$%, ", "");
            var model = new EditPhotoModel()
            {
                Id = image.Id,
                Title = image.Title,
                Tags = imgT,
                GeoLocation = image.GeoLocation
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditImage(int id, string title, string tags, string geoLocation)
        {


            await _imageService.EditImage(id, title, tags, geoLocation);



            return RedirectToAction("Index", "Gallery");
        }

        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = _imageService.GetById(id);
            Uri uri = new Uri(image.Url);
            var container = _imageService.GetBlobContainer(AzureConnectionString, "photodb");


            string filename = Path.GetFileName(uri.LocalPath);

            var blob = container.GetBlockBlobReference(filename);

            await _imageService.DelImage(id);
            await blob.DeleteIfExistsAsync();



            return RedirectToAction("Index", "Gallery");
        }


        public IActionResult SharePhoto(int photoId, string userId, string userName)
        {

            var image = _imageService.GetById(photoId);
            var model = new ShareModel()
            {
                Url = image.Url,
                UserName = userName,
                ImgId= photoId,
                UserId= userId
            };


            return View(model);
        }

        public async Task<IActionResult> ConfShareImage(int photoId, string userId)
        {
            var byId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _imageService.ShareImage(photoId, byId, userId);

            return RedirectToAction("Index", "Gallery");
        }


        [HttpPost]
        public async Task<IActionResult> AddToAlbum(int photoId, int albumId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _imageService.AddToAlbums(userId, photoId, albumId);



            return RedirectToAction("Album", "Gallery", albumId);
        }

    }
}
