using System.Threading.Tasks;

namespace BeavisCli.Jobs
{
    public class ContinueCommandJob : IJob
    {
        private readonly CommandContext _context;
        private readonly CommandSegment _next;

        public ContinueCommandJob(CommandContext context, CommandSegment next)
        {
            _context = context;
            _next = next;
        }

        public async Task RunAsync(JobContext context)
        {
            _context.OnRequestChanged(context.HttpContext, context.Response);

            var reader = new CommandSegmentInput(context.HttpContext, () => context.Content);

            await _next(reader, _context);
        }
    }
}
