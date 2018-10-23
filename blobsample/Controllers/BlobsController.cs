using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace blobsample.Controllers
{
    public class BlobsController : Controller
    {
        // GET: Blobs
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UploadBlob(HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    CloudBlobContainer container = GetCloudBlobContainer();
                    CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
                    using (var fileStream = file.InputStream)
                    {
                        blob.UploadFromStream(fileStream);
                    }
                }
            }
            TempData["message"] = "Upload file success";
            return RedirectToAction("Index");
        }

        public ActionResult ListBlobs()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            List<BlobItem> blobs = new List<BlobItem>();

            foreach (IListBlobItem item in container.ListBlobs())
            {
                CloudBlockBlob blob = (CloudBlockBlob)item;

                var readPolicy = new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(5)
                };

                var sasConstraints = new SharedAccessBlobPolicy();
                sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
                sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(10);
                sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

                var sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

                var newUri = new Uri(blob.Uri.AbsoluteUri + blob.GetSharedAccessSignature(readPolicy));

                blobs.Add(new BlobItem()
                {
                    FileName = blob.Name,
                    Uri = newUri.ToString()
                });
            }

            return View(blobs);
        }

        public string DownloadBlob(string filename)
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference("myBlob");
            using (var fileStream = System.IO.File.OpenWrite(@"user local folder + file name"))
            {
                blob.DownloadToStream(fileStream);
            }
            return "success!";
        }

        public ActionResult DeleteBlob(string filename)
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(filename);
            blob.Delete();

            return RedirectToAction("ListBlobs");
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("pc532_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("aidata");
            return container;
        }

    }

    public class BlobItem
    {
        public String FileName { get; set; }
        public String Uri { get; set; }

    }
}