using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AzureStorageLibrary;
using AzureStorageLibrary.Models;
using AzureStorageLibrary.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace WatermarkProcessFunction
{
    public static class Function
    {
        [FunctionName("Function")]
        public async static Task Run([QueueTrigger("watermarkqueue")] PictureWatermarkQueue pictureWatermarkQueue, ILogger log)
        {
            ConnectionStrings.AzureStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=udemyrealstorageaccount;AccountKey=kCxkM3J1A4DQIhoWDZVXr0JbXIaiv43oBDsjXqzEHYGJ3piMPRRXJpov1zFzjaniERUKEUxBZhTG+AStDsIJHA==;EndpointSuffix=core.windows.net";

            IBlobStorage blobStorage = new BlobStorage();
            INoSqlStorage<UserPicture> noSqlStorage = new NoSqlStorage<UserPicture>();

            foreach (var item in pictureWatermarkQueue.WatermarkPicture)
            {
                using var stream = await blobStorage.DownloadAsync(item, EContainerName.pictures);
                using var ms = AddWatermark(pictureWatermarkQueue.WatermarkText, stream);

                await blobStorage.UploadAsync(ms, item, EContainerName.watermarkpictures);

                log.LogInformation($"{item} resmine watermark eklenmiştir.");
            }

            var userPicture = await noSqlStorage.GetAsync(pictureWatermarkQueue.UserId, pictureWatermarkQueue.City);
            if (userPicture.WatermarkRawPaths != null)
            {
                pictureWatermarkQueue.WatermarkPicture.AddRange(userPicture.WatermarkPaths);
            }
            userPicture.WatermarkPaths = pictureWatermarkQueue.WatermarkPicture;

            await noSqlStorage.AddAsync(userPicture);

            HttpClient client = new HttpClient();
            var responce = await client.GetAsync("https://localhost:44327/api/Notifications/CompleteWatermarkProcess/" + pictureWatermarkQueue.ConnectionId);

            log.LogInformation($"Client {pictureWatermarkQueue.ConnectionId} bilgilendirilmiştir.");
        }

        public static MemoryStream AddWatermark(string watermarkText, Stream pictureStream)
        {
            var ms = new MemoryStream();
            using (var image = Bitmap.FromStream(pictureStream))
            {
                using (var tempBitmap = new Bitmap(image.Width, image.Height))
                {
                    using (var gph = Graphics.FromImage(tempBitmap))
                    {
                        gph.DrawImage(image, 0, 0);

                        var font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold);
                        var color = Color.FromArgb(255, 0, 0);
                        var brush = new SolidBrush(color);
                        var point = new Point(20, image.Height - 50);

                        gph.DrawString(watermarkText, font, brush, point);

                        tempBitmap.Save(ms, ImageFormat.Png);
                    }
                }
            }

            ms.Position = 0;
            return ms;
        }
    }
}