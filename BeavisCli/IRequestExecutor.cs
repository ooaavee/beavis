using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface IRequestExecutor
    {
        Task ExecuteAsync(WebCliRequest request, WebCliResponse response, HttpContext httpContext);
    }
}
