using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BeavisCli.Services
{
    public class JobPool : IJobPool
    {
        private readonly ConcurrentDictionary<string, IJob> _jobs = new ConcurrentDictionary<string, IJob>();
        private readonly ILogger<JobPool> _logger;

        public JobPool(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<JobPool>();
        }

        /// <summary>
        /// Adds a job and returns its identifier. 
        /// </summary>
        public string Push(IJob job)
        {
            string key = KeyUtil.GenerateKey(s => _jobs.ContainsKey(s));

            if (!_jobs.TryAdd(key, job))
            {
                _logger.LogError($"Unable to push a new job by using the key '{key}'.");
                throw new InvalidOperationException($"Unable to push a new job by using the key '{key}'.");
            }

            return key;
        }

        /// <summary>
        /// Runs the job.
        /// </summary>
        public async Task RunAsync(string key, JobContext context)
        {
            // find the job and remove it from the pool
            if (!_jobs.TryRemove(key, out IJob job))
            {
                _logger.LogError($"Unable to find a job by using the key '{key}'.");
                throw new InvalidOperationException($"Unable to find a job by using the key '{key}'.");
            }

            // run the job
            try
            {
                _logger.LogDebug($"Executing a job by using the key '{key}'.");
                await job.RunAsync(context);
                _logger.LogDebug($"Completing a job execution by using the key '{key}'.");
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while executing a job by using the key '{key}'.", e);
                throw;
            }
        }
    }
}
