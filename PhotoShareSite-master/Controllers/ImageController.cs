using ImageGallery.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PhotoShareSite.Models;
using System.Net.Http.Headers;
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
        public IActionResult Upload()
        {
            var model = new UploadImageModel();
            return View(model);
        }

        [HttpPost]
       
    public async Task<IActionResult> UploadNewImage(IFormFile file, string title, string tags)
    {
            

            var container = _imageService.GetBlobContainer(AzureConnectionString, "photodb");
        var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
        var fileName = content.FileName.Trim('"');

        var blob = container.GetBlockBlobReference(fileName);
        await blob.UploadFromStreamAsync(file.OpenReadStream());
        await _imageService.SetImage(title, tags, blob.Uri);



        return RedirectToAction("Index", "Gallery");
    }
}
}
