using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("files", "A tool for managing files in the file storage.")]
    public class Files : ICommand
    {
        public async Task ExecuteAsync(CommandBuilder builder, CommandContext context)
        {
            // options
            IOption ls = builder.Option("-ls", "Lists all files.", OptionType.NoValue);
            IOption remove = builder.Option("-rm", "Removes a file by using the file id.");
            IOption removeAll = builder.Option("-ra", "Removes all files.", OptionType.NoValue);
            IOption download = builder.Option("-download", "Downloads a file by using the file id.");

            await context.OnExecuteAsync(async () =>
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
                    string id = remove.Value();
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
                    string id = download.Value();
                    await Download(context, id);
                    succeed = true;
                }

                if (!succeed)
                {
                    return await context.ExitWithHelpAsync();
                }

                return await context.ExitAsync();
            });
        }

        /// <summary>
        /// Lists all files.
        /// </summary>
        private static async Task ListFiles(CommandContext context)
        {
            IFileStorage files = context.HttpContext.RequestServices.GetRequiredService<IFileStorage>();

            var items = new List<FileListModel>();

            foreach (Tuple<string, FileContent> item in await files.GetAllAsync())
            {
                items.Add(new FileListModel
                {
                    FileId = item.Item1,
                    Type = item.Item2.Type,
                    Name = item.Item2.Name
                });
            }

            if (items.Any())
            {
                context.WriteObjects(items, x => x.FileId, x => x.Type, x => x.Name, false, true);
            }
            else
            {
                context.WriteText("No files found.");
            }
        }

        /// <summary>
        /// Removes a file by using the file id.
        /// </summary>
        private static async Task Remove(CommandContext context, string id)
        {
            IFileStorage files = context.HttpContext.RequestServices.GetRequiredService<IFileStorage>();

            FileContent file = await files.RemoveAsync(id);

            if (file != null)
            {
                context.WriteText("File removed.");
            }
            else
            {
                context.WriteError($"Unable to find a file to remove by using the id '{id}'.");
            }
        }

        /// <summary>
        /// Removes all files.
        /// </summary>
        private static async Task RemoveAll(CommandContext context)
        {
            IFileStorage files = context.HttpContext.RequestServices.GetRequiredService<IFileStorage>();

            int count = await files.RemoveAllAsync();

            if (count == 0)
            {
                context.WriteError("Unable to find files to remove.");
            }
            else
            {
                context.WriteText($"Removed {count} file(s).");
            }
        }

        /// <summary>
        /// Downloads a file by using the file id.
        /// </summary>
        private static async Task Download(CommandContext context, string id)
        {
            IFileStorage files = context.HttpContext.RequestServices.GetRequiredService<IFileStorage>();

            FileContent file = await files.GetAsync(id);

            if (file != null)
            {
                context.WriteFile(file.GetBytes(), file.Name, file.Type);
            }
            else
            {
                context.WriteError($"Unable to find a file to download by using the id '{id}'.");
            }
        }

        private class FileListModel
        {
            public string FileId { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
        }
    }
}
