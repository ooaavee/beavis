using System;
using System.Threading.Tasks;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Internal
{
    internal class RequestHandler
    {
        private readonly ApplicationSpace _space;

        public RequestHandler(ApplicationSpace space)
        {
            _space = space;
        }

        public async Task HandleAsync(TerminalRequest request, HttpContext httpContext)
        {
            CliContext context = new CliContext(request, new TerminalResponse(), httpContext);

            try
            {
                AbstractApplication app = _space.FindApplicaton(context);

                await app.ExecuteAsync(context);
            }
            catch (ApplicationSpaceException ex)
            {
                using (context.Response.BeginInteraction())
                {
                    context.Response.WriteError(ex);
                }
            }
            catch (CommandParsingException ex)
            {
                using (context.Response.BeginInteraction())
                {
                    context.Response.WriteError(ex);
                }
            }
            catch (Exception ex)
            {
                //
                // TODO: Pitäisikö tässä palauttaa käyttöliittymälle koko stack trace
                //
                using (context.Response.BeginInteraction())
                {
                    context.Response.WriteError(ex);
                }
            }

        }
    }
}