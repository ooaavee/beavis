using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public delegate Task<CommandResult> CommandSegment(CommandSegmentInput input, CommandContext context);

    public class CommandSegmentInput
    {
        private readonly HttpContext _httpContext;
        private readonly Func<string> _read;

        public CommandSegmentInput(HttpContext httpContext, Func<string> read)
        {
            _httpContext = httpContext;
            _read = read;
        }

        public string GetString()
        {
            return _read();
        }

        public bool GetBoolean()
        {
            throw new NotImplementedException();            
        }

    }
}


