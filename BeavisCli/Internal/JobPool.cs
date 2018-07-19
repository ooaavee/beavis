using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BeavisCli.Internal
{
    internal class JobPool
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
            string key = ShortKey.Create(s => _jobs.ContainsKey(s));

            if (!_jobs.TryAdd(key, job))
            {
                _logger.LogError($"Unable to push a new job by using the key '{key}'.");
                throw new InvalidOperationException($"Unable to push a new job by using the key '{key}'.");
            }

            return key;
        }

        /// <summary>
        /// Finds a job from the job pool by its identifier and removes it from the job pool.
        /// </summary>
        public async Task RunAsync(string key, HttpContext context, WebCliResponse response)
        {
            IJob job;

            // find the job and remove it from the memory
            if (!_jobs.TryRemove(key, out job))
            {
                _logger.LogError($"Unable to find a job by using the key '{key}'.");
                throw new InvalidOperationException($"Unable to find a job by using the key '{key}'.");
            }

            // run the job
            try
            {
                _logger.LogDebug($"Executing a job by using the key '{key}'.");
                await job.RunAsync(context, response);
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
