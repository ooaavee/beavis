using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BeavisCli.Demo.Applications.Utils;

namespace BeavisCli.Demo.Applications
{
    [WebCliApplication("types", "This sample application finds types available in the current AppDomain.")]
    public class Types : WebCliApplication
    {
        public override async Task ExecuteAsync(WebCliContext context)
        {
            IOption full = context.Option("-fn", "Display full type name.", CommandOptionType.NoValue);

            IOption filter = context.Option("-f", "Filter types by name.", CommandOptionType.SingleValue);

            await OnExecuteAsync(() =>
            {
                var types = new List<Type>();

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        types.AddRange(GetTypes(assembly));
                    }
                    catch (Exception)
                    {
                        //Debugger.Break();
                    }
                }

                var values = new List<string>();

                string filterText = filter.HasValue() ? filter.Value() : null;

                foreach (Type type in types)
                {
                    string name = TypeUtil.GetFriendlyName(type, full.HasValue());

                    if (filterText == null || name.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        values.Add(name);
                    }
                }

                values.Sort();

                bool truncated = false;

                var results = ResponseRenderer.FormatLines(values, true).ToList();

                const int maxResult = 200;

                int count = results.Count;

                if (count > maxResult)
                {
                    results = results.GetRange(0, maxResult);
                    truncated = true;
                }

                if (results.Any())
                {

                    if (truncated)
                    {
                        context.Response.WriteInformation($"Showing first {maxResult} results.");
                        context.Response.WriteInformation($"Use '-f' option to filter types by name.");
                    }
                    else
                    {
                        context.Response.WriteInformation($"Found {count} types:");

                    }

                    context.Response.WriteInformation(results);
                }
                else
                {
                    context.Response.WriteInformation("No types found.");
                }




                return Exit(context);
            }, context);
        }


        private IEnumerable<Type> GetTypes(Assembly assembly)
        {
            foreach (var candidate in assembly.ExportedTypes)
            {
                yield return candidate;
            }

            //    foreach (var candidate in assembly.ExportedTypes.Select(t => t.GetTypeInfo()))
            //    {
            //        yield return candidate;
            //    }
            //}
        }
    }
}
