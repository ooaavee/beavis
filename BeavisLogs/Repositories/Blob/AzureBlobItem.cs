using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BeavisLogs.Repositories.Blob
{
    public class AzureBlobItem
    {
        public AzureBlobItem(IListBlobItem item)
        {
            Item = item;
        }

        public IListBlobItem Item { get; }

        public bool IsBlockBlob => Item.GetType() == typeof(CloudBlockBlob);
        public bool IsPageBlob => Item.GetType() == typeof(CloudPageBlob);
        public bool IsDirectory => Item.GetType() == typeof(CloudBlobDirectory);

        public string BlobName => IsBlockBlob ? ((CloudBlockBlob)Item).Name :
                                  IsPageBlob ? ((CloudPageBlob)Item).Name : 
                                  IsDirectory ? ((CloudBlobDirectory)Item).Prefix : 
                                  "";
        
        public string Folder => BlobName.Contains("/") ? 
                                    BlobName.Substring(0, BlobName.LastIndexOf("/", StringComparison.Ordinal)) : "";

        public string Name => BlobName.Contains("/") ? 
                                    BlobName.Substring(BlobName.LastIndexOf("/", StringComparison.Ordinal) + 1) : BlobName;
    }
}
