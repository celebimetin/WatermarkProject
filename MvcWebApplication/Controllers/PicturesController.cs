using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using AzureStorageLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MvcWebApplication.Controllers
{
    public class PicturesController : Controller
    {
        public string UserId { get; set; } = "12345";
        public string City { get; set; } = "istanbul";

        private readonly INoSqlStorage<UserPicture> _noSqlStorage;
        private readonly IBlobStorage _blobStorage;

        public PicturesController(INoSqlStorage<UserPicture> noSqlStorage, IBlobStorage blobStorage)
        {
            _noSqlStorage = noSqlStorage;
            _blobStorage = blobStorage;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.userId = UserId;
            ViewBag.city = City;

            var fileBlobs = new List<FileBlob>();
            var user = await _noSqlStorage.GetAsync(UserId, City);

            if (user != null)
            {
                user.Paths.ForEach(x =>
                {
                    fileBlobs.Add(new FileBlob { Name = x, Url = $"{_blobStorage.BlobUrl}/{EContainerName.pictures}/{x}" });
                });
            }
            ViewBag.fileBlobs = fileBlobs;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IEnumerable<IFormFile> pictures)
        {
            var picturesList = new List<string>();
            foreach (var picture in pictures)
            {
                var newPictureName = $"{Guid.NewGuid()}{Path.GetExtension(picture.FileName)}";
                await _blobStorage.UploadAsync(picture.OpenReadStream(), newPictureName, EContainerName.pictures);
                picturesList.Add(newPictureName);
            }

            var isUser = await _noSqlStorage.GetAsync(UserId, City);
            if (isUser != null)
            {
                picturesList.AddRange(isUser.Paths);
                isUser.Paths = picturesList;
            }
            else
            {
                isUser = new UserPicture();
                isUser.RowKey = UserId;
                isUser.PartitionKey = City;
                isUser.Paths = picturesList;
            }
            await _noSqlStorage.AddAsync(isUser);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddWatermark(PictureWatermarkQueue pictureWatermark)
        {
            var jsonString = JsonConvert.SerializeObject(pictureWatermark);
            var jsonStringBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

            AzureQueue azureQueue = new AzureQueue("watermarkqueue");
            await azureQueue.SendMessageAsync(jsonStringBase64);

            return Ok();
        }

        public async Task<IActionResult> ShowWatermark()
        {
            var fileBlobs = new List<FileBlob>();
            var userPicture = await _noSqlStorage.GetAsync(UserId, City);
            userPicture.WatermarkPaths.ForEach(x =>
            {
                fileBlobs.Add(new FileBlob { Name = x, Url = $"{_blobStorage.BlobUrl}/{EContainerName.watermarkpictures}/{x}" });
            });

            ViewBag.fileBlobs = fileBlobs;
            return View();
        }
    }
}