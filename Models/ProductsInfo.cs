
using Azure;

namespace RetailStore.Models
{
    public class ProductsInfo
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ETag ETag { get; set; }

        // BlobName stores the name of the blob (image) in Azure Blob Storage
        public string BlobName { get; set; }

        // ImageUrl will hold the full URL of the image from Blob Storage
        public string ImageUrl { get; set; }
    }
}
