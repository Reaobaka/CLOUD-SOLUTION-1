using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Azure.Data.Tables;
using RetailStore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;
using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Files.Shares;

namespace RetailStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ShareServiceClient _shareServiceClient;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            _blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("AzureBlobStorage"));
            _shareServiceClient = new ShareServiceClient(_configuration.GetConnectionString("AzureFileShare"));
        }


        public IActionResult Index()
        {
            return View();
        }


        

        public IActionResult Products()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadBlob(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("File not selected");

            var containerClient =
_blobServiceClient.GetBlobContainerClient("products");
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(file.FileName);
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return RedirectToAction("Products");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadBlob(string blobName)
        {
            var containerClient =
_blobServiceClient.GetBlobContainerClient("products");

            var blobClient = containerClient.GetBlobClient(blobName);
            var download = await blobClient.DownloadAsync();

            return File(download.Value.Content, "application/octet-stream", blobName);
        }

        

        public async Task<IActionResult> ListFiles()
        {
            try
            {
                var shareClient = _shareServiceClient.GetShareClient("logsandpolicies");
                var directoryClient = shareClient.GetRootDirectoryClient();

                // Ensure the share and directory exist
                await shareClient.CreateIfNotExistsAsync();

                var fileItems = directoryClient.GetFilesAndDirectoriesAsync();

                var fileList = new List<string>();

                await foreach (var fileItem in fileItems)
                {
                    if (!fileItem.IsDirectory) // Only add files, skip directories
                    {
                        fileList.Add(fileItem.Name);
                    }
                }

                // If no files were found, display a message in the view
                if (fileList.Count == 0)
                {
                    ViewBag.Message = "No files found in Azure File Share.";
                }

                return View(fileList); // Passing the list to the view
            }
            catch (Exception ex)
            {
                // Log the error and show an error message in the view
                ViewBag.ErrorMessage = $"Error retrieving files: {ex.Message}";
                return View(new List<string>()); // Return an empty list in case of an error
            }
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("File not selected");

            try
            {
                var shareClient = _shareServiceClient.GetShareClient("logsandpolicies");
                await shareClient.CreateIfNotExistsAsync();

                var directoryClient = shareClient.GetRootDirectoryClient();
                var fileClient = directoryClient.GetFileClient(file.FileName);

                using (var stream = file.OpenReadStream())
                {
                    await fileClient.CreateAsync(file.Length);
                    await fileClient.UploadAsync(stream);
                }

                ViewBag.Message = "File uploaded successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error uploading file: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                ViewBag.ErrorMessage = "File name cannot be empty.";
                return RedirectToAction("Index");
            }

            try
            {
                var shareClient = _shareServiceClient.GetShareClient("logsandpolicies");
                var directoryClient = shareClient.GetRootDirectoryClient();
                var fileClient = directoryClient.GetFileClient(fileName);

                var downloadResponse = await fileClient.DownloadAsync();

                return File(downloadResponse.Value.Content, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error downloading file: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
    }




        


