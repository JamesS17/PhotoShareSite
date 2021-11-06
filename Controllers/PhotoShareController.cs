using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoShareSite.Controllers
{
    public class PhotoShareController : Controller
    {
        static CloudBlobClient _blobClient;
        const string _blobContainerName = "photodb";
        private readonly IConfiguration _configuration;
        static CloudBlobContainer _blobContainer;

        public PhotoShareController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var storageConnectionString = _configuration.GetValue<string>("StorageConnectionString");
                var storeageAccount = CloudStorageAccount.Parse(storageConnectionString);

                _blobClient = storeageAccount.CreateCloudBlobClient();
                _blobContainer = _blobClient.GetContainerReference(_blobContainerName);
                await _blobContainer.CreateIfNotExistsAsync();

                await _blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);


                List<Uri> allBlobs = new List<Uri>();
                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    var response = await _blobContainer.ListBlobsSegmentedAsync(blobContinuationToken);
                    foreach (IListBlobItem blob in response.Results)
                    {
                        if (blob.GetType() == typeof(CloudBlockBlob))
                        {
                            allBlobs.Add(blob.Uri);
                        }
                    }
                } while (blobContinuationToken != null);

                return View(allBlobs);
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadAsync()
        {
            try
            {
                var request = await HttpContext.Request.ReadFormAsync();
                if (request.Files == null)
                {
                    return BadRequest("Could not upload files");
                }
                var files = request.Files;
                if (files.Count == 0)
                {
                    return BadRequest("Could not upload empty files");
                }

                for (int i = 0; i < files.Count; i++)
                {
                    var blob = _blobContainer.GetBlockBlobReference(GetRandomBlobName(files[i].FileName));
                    using (var stream = files[i].OpenReadStream())
                    {
                        await blob.UploadFromStreamAsync(stream);

                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteImage(string name)
        {
            try
            {
                Uri uri = new Uri(name);
                string filename = Path.GetFileName(uri.LocalPath);

                var blob = _blobContainer.GetBlockBlobReference(filename);
                await blob.DeleteIfExistsAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }


        [HttpPost]
        public async Task<ActionResult> DeleteAll()
        {
            try
            {

                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    var response = await _blobContainer.ListBlobsSegmentedAsync(blobContinuationToken);
                    foreach (IListBlobItem blob in response.Results)
                    {
                        if (blob.GetType() == typeof(CloudBlockBlob))
                        {
                            await ((CloudBlockBlob)blob).DeleteIfExistsAsync();
                        }
                    }
                } while (blobContinuationToken != null);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        private string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }
    }
}
