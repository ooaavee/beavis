using System;

namespace BeavisLogs.Repositories.Blob
{
    public class AzureBlobSettings
    {
        public AzureBlobSettings(string connectionString, string containerName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }

            ConnectionString = connectionString;
            ContainerName = containerName;
        }

        public string ConnectionString { get; }
        public string ContainerName { get; }
    }
}
