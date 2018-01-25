﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface IJob
    {
        Task ExecuteAsync(HttpContext context, WebCliResponse response);
    }
}