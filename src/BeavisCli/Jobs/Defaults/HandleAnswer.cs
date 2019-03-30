using BeavisCli.JsInterop.Statements;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BeavisCli.Jobs.Defaults
{
    public class HandleAnswer : IJob
    {
        private readonly AnswerType _type;
        private readonly BeavisCliOptions _options;
        private readonly StringAnswerHandler _stringAnswerHandler;
        private readonly BooleanAnswerHandler _booleanAnswerHandler;
        private readonly CommandContext _context;

        public HandleAnswer(CommandContext context, StringAnswerHandler handler)
        {
            _type = AnswerType.String;
            _options = context.HttpContext.RequestServices.GetRequiredService<IOptions<BeavisCliOptions>>().Value;
            _stringAnswerHandler = handler;
            _context = context;
        }

        public HandleAnswer(CommandContext context, BooleanAnswerHandler handler)
        {
            _type = AnswerType.Boolean;
            _options = context.HttpContext.RequestServices.GetRequiredService<IOptions<BeavisCliOptions>>().Value;
            _booleanAnswerHandler = handler;
            _context = context;
        }

        public async Task RunAsync(JobContext context)
        {
            _context.OnRequestChanged(context.HttpContext, context.Response);

            // restore terminal default prompt
            _context.WriteJs(new SetPrompt(_options.Prompt));

            // set terminal history back to 'enabled'        
            _context.WriteJs(new SetHistory(true));

            switch (_type)
            {
                case AnswerType.String:
                    await _stringAnswerHandler(context.Content, _context);
                    break;

                case AnswerType.Boolean:
                    await _booleanAnswerHandler(ConvertToBoolean(context.Content), _context);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool ConvertToBoolean(string answer)
        {
            // Only 'yes' will be accepted to approve.
            bool yes = answer.ToLowerInvariant().Trim() == "yes";
            return yes;
        }

        private enum AnswerType
        {
            String,
            Boolean
        }
    }
}
