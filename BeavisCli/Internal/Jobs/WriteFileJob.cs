﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Internal.Jobs
{
    internal class WriteFileJob : IJob
    {
        private readonly byte[] _data;
        private readonly string _fileName;
        private readonly string _mimeType;

        public WriteFileJob(byte[] data, string fileName, string mimeType)
        {
            _data = data;
            _fileName = fileName;
            _mimeType = mimeType;
        }

        public Task ExecuteAsync(HttpContext context, WebCliResponse response)
        {
            response.AddStatement(new DownloadJs(_data, _fileName, _mimeType));
            return Task.CompletedTask;
        }
    }
}