using BeavisLogs.Repositories.Blob;
using BeavisLogs.Services;

namespace BeavisLogs.Repositories
{
    public class RepositoryFactory
    {
        private readonly ConfigurationAccessor _configuration;

        public RepositoryFactory(ConfigurationAccessor configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration["AccessRepository:ConnectionString"];
        }

        private string GetContainerName()
        {
            return _configuration["AccessRepository:ContainerName"];
        }

        public IAccessRepository Create()
        {
            return Create(GetConnectionString());
        }

        public IAccessRepository Create(string connectionString)
        {
            return Create(new AzureBlobSettings(connectionString, GetContainerName()));
        }

        private IAccessRepository Create(AzureBlobSettings settings)
        {   
            IAzureBlobStorage storage = new AzureBlobStorage(settings);
            IAccessRepository repository = new AccessRepository(storage);
            return repository;
        }
    }
}
