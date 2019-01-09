using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BeavisCli;
using BeavisCli.Jobs;
using BeavisLogs.Models.DataSources;
using BeavisLogs.Models.Logs;
using BeavisLogs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisLogs.Drivers.Renderers
{
    public class FileRenderer : LogEventOutputRenderer
    {
        private readonly Dictionary<string, string> _files = new Dictionary<string, string>();

        private readonly FileRenderBehaviour _behaviour;

        public FileRenderer(DataSourceInfo[] sources, FileRenderBehaviour behaviour)
        {
            _behaviour = behaviour;

            string rootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(rootPath);

            foreach (var source in sources)
            {
                var path = Path.Combine(rootPath, source.Id);
                File.Create(path).Dispose();
                _files.Add(source.Id, path);
            }
        }

        public override int MaxBlockSize => int.MaxValue;

        public override void OnServingStarted(DataSourceInfo source, HttpContext context, Response response)
        {
        }

        public override void Output(DataSourceInfo source, ILogEvent[] events, HttpContext context, Response response)
        {
            LogEventFormatter formatter = context.RequestServices.GetRequiredService<LogEventFormatter>();

            List<string> lines = new List<string>();

            foreach (ILogEvent e in events)
            {
                string text = formatter.Format(e);
                lines.Add(text);
            }

            string path = _files[source.Id];

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        public override void Output(DataSourceInfo source, Exception[] exceptions, HttpContext context, Response response)
        {
            LogEventFormatter formatter = context.RequestServices.GetRequiredService<LogEventFormatter>();


            // TODO: Kirjoita teksti tiedotoon!
            //throw new NotImplementedException();
        }

        public override void OnLimitReached(HttpContext context, Response response)
        {
            //    throw new NotImplementedException();
        }

        public override void OnPollingCompleted(HttpContext context, Response response)
        {

            if (_behaviour == FileRenderBehaviour.SingleFiles)
            {
                foreach (var file in _files)
                {
                    byte[] content = File.ReadAllBytes(file.Value);
                    IJob job = new WriteFileJob(content, $"{file.Key}.log", "text/plain");
                    JobScheduler.New(job, response, context);
                }
            }
            else if (_behaviour == FileRenderBehaviour.Zip)
            {

            }
            else
            {
                throw new InvalidOperationException("Behaviour not supported.");
            }


            // TODO: Tee fileistä ZIPi ja lähetä ne clientielli
            //    throw new NotImplementedException();
        }




    }


    public enum FileRenderBehaviour
    {
        SingleFiles,
        Zip
    }
}