using System.Collections.Generic;

namespace AzureStorageLibrary.Models
{
    public class PictureWatermarkQueue
    {
        public string UserId { get; set; }
        public string City { get; set; }
        public List<string> WatermarkPicture { get; set; }
        public string ConnectionId { get; set; }
        public string WatermarkText { get; set; }
    }
}