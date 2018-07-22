using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeavisCli.JavaScriptStatements;

namespace BeavisCli.Internal.Applications
{
    [WebCliApplication("filestorage", "Manage file storage.")]
    internal class FileStorage : WebCliApplication
    {
        private readonly IFileStorage _files;

        public FileStorage(IFileStorage files)
        {
            _files = files;
        }

        public override async Task ExecuteAsync(WebCliContext context)
        {
            // options
            IOption ls = context.Option("-ls", "Lists all files.", CommandOptionType.NoValue);
            IOption remove = context.Option("-rm", "Removes a file by using the file id.", CommandOptionType.SingleValue);
            IOption removeAll = context.Option("-ra", "Removes all files.", CommandOptionType.NoValue);
            IOption download = context.Option("-download", "Downloads a file by using the file id.", CommandOptionType.SingleValue);

            await OnExecuteAsync(async () =>
            {
                bool succeed = false;

                // lists all files
                if (ls.HasValue())
                {
                    await ListFiles(context);
                    succeed = true;
                }

                // removes a file by using the file id
                else if (remove.HasValue())
                {
                    FileId id = new FileId(remove.Value());
                    await Remove(context, id);
                    succeed = true;
                }

                // removes all files
                else if (removeAll.HasValue())
                {
                    await RemoveAll(context);
                    succeed = true;
                }

                // downloads a file by using the file id
                else if (download.HasValue())
                {
                    FileId id = new FileId(download.Value());
                    await Download(context, id);
                    succeed = true;
                }

                if (!succeed)
                {
                    return await ExitWithHelpAsync(context);
                }

                return await ExitAsync(context);
            }, context);
        }

        /// <summary>
        /// Lists all files.
        /// </summary>
        private async Task ListFiles(WebCliContext context)
        {
            var items = new List<Tuple<string, string, string>>();

            foreach (Tuple<FileId, FileContent> item in await _files.GetAllAsync())
            {
                items.Add(new Tuple<string, string, string>(item.Item1.Value, item.Item2.Type, item.Item2.Name));
            }

            if (items.Any())
            {
                items.Insert(0, new Tuple<string, string, string>("FILE ID", "TYPE", "NAME"));

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

        /// <summary>
        /// Removes a file by using the file id.
        /// </summary>
        private async Task Remove(WebCliContext context, FileId id)
        {
            FileContent file = await _files.RemoveAsync(id);

            if (file != null)
            {
                context.Response.WriteInformation("File removed.");
            }
            else
            {
                context.Response.WriteError($"Unable to find a file to remove by using the id '{id}'.");
            }
        }

        /// <summary>
        /// Removes all files.
        /// </summary>
        private async Task RemoveAll(WebCliContext context)
        {
            int count = await _files.RemoveAllAsync();

            if (count == 0)
            {
                context.Response.WriteError("Unable to find files to remove.");
            }
            else
            {
                context.Response.WriteInformation($"Removed {count} file(s).");
            }
        }

        /// <summary>
        /// Downloads a file by using the file id.
        /// </summary>
        private async Task Download(WebCliContext context, FileId id)
        {
            FileContent file = await _files.GetAsync(id);

            if (file != null)
            {
                IJavaScriptStatement js = new DownloadJs(file.GetBytes(), file.Name, file.Type);
                context.Response.AddJavaScript(js);
            }
            else
            {
                context.Response.WriteError($"Unable to find a file to download by using the id '{id}'.");
            }
        }

    }
}
