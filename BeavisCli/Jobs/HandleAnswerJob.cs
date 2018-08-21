using System;
using System.Threading.Tasks;
using BeavisCli.JavaScriptStatements;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BeavisCli.Jobs
{
    public class HandleAnswerJob : IJob
    {
        private readonly BeavisCliOptions _options;
        private readonly HandlerTypes _type;
        private readonly CommandContext _context;
        private readonly StringAnswerHandler _s;
        private readonly BooleanAnswerHandler _b;

        public HandleAnswerJob(CommandContext context, StringAnswerHandler handler)
        {
            _options = context.HttpContext.RequestServices.GetRequiredService<IOptions<BeavisCliOptions>>().Value;
            _type = HandlerTypes.String;
            _context = context;
            _s = handler;
        }

        public HandleAnswerJob(CommandContext context, BooleanAnswerHandler handler)
        {
            _options = context.HttpContext.RequestServices.GetRequiredService<IOptions<BeavisCliOptions>>().Value;
            _type = HandlerTypes.Boolean;
            _context = context;
            _b = handler;
        }

        public async Task RunAsync(JobContext context)
        {
            string answer = context.Content;

            _context.OnRequestChanged(context.HttpContext, context.Response);

            _context.WriteJs(new SetPrompt(_options.Prompt));
            _context.WriteJs(new SetHistory(true));

            switch (_type)
            {
                case HandlerTypes.String:
                    await _s(answer, _context);
                    break;

                case HandlerTypes.Boolean:
                    await _b(StringToBoolean(answer), _context);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private static bool StringToBoolean(string answer)
        {
            return false;
        }

        private enum HandlerTypes
        {
            String, Boolean
        }
    }
}
