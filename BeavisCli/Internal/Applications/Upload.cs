using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{
    [WebCliApplication("upload", "Uploads a file.")]
    internal class Upload : WebCliApplication
    {
        private readonly IFileStorage _files;

        public Upload(IFileStorage files)
        {
            _files = files;
        }

        public override async Task ExecuteAsync(WebCliContext context)
        {
            // options
            IOption list = context.Option("-l", "Lists all uploaded files from the file storage.", CommandOptionType.NoValue);
            IOption remove = context.Option("-r", "Removes an uploaded file from the file storage by using the file id.", CommandOptionType.SingleValue);
            IOption removeAll = context.Option("-ra", "Removes all uploaded files from the file storage.", CommandOptionType.NoValue);

            await OnExecuteAsync(async () =>
            {
                // lists all uploaded files from the file storage
                if (list.HasValue())
                {
                    var items = new List<Tuple<string, string, string>>();

                    foreach (Tuple<string, UploadedFile> item in await _files.GetAllAsync())
                    {
                        var t = new Tuple<string, string, string>(item.Item1, item.Item2.Type, item.Item2.Name);
                        items.Add(t);
                    }

                    if (items.Any())
                    {
                        var t = new Tuple<string, string, string>("FILE ID", "TYPE", "NAME");
                        items.Insert(0, t);

                        foreach (string line in ResponseRenderer.FormatLines(items, true))
                        {
                            context.Response.WriteInformation(line);
                        }
                    }
                    else
                    {
                        context.Response.WriteInformation("No files found.");
                    }
                }

                // removes an uploaded file from the file storage by using the file id
                if (remove.HasValue())
                {
                    string input = remove.Value();

                    string id = ShortKey.FindMatching(input, delegate
                    {

                        return null;
                    });

                }

                // removes all uploaded files from the file storage
                if (removeAll.HasValue())
                {

                }


                return Exit(context).Result;


            }, context);
        }
    }
}
