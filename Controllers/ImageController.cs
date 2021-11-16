using ImageGallery.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PhotoShareSite.Models;
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
}
}
