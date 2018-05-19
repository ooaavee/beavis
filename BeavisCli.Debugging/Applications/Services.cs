using BeavisCli.Debugging.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BeavisCli.Debugging.Applications
{
    [WebCliApplicationDefinition(Name = "services", Description = "Lists all services available.")]
    public class Services : WebCliApplication
    {
        public override async Task ExecuteAsync(WebCliContext context)
        {
            IOption full = context.Option("-f", "Display fully type names.", CommandOptionType.NoValue);

            await OnExecuteAsync(() =>
            {
                // get all services
                IServiceProvider serviceProvider = context.HttpContext.RequestServices;
                Type type = serviceProvider.GetType();
                PropertyInfo property = type.GetProperty("RealizedServices", BindingFlags.Instance | BindingFlags.NonPublic);
                object value = property.GetValue(serviceProvider);
                ConcurrentDictionary<Type, Func<ServiceProvider, object>> items = (ConcurrentDictionary<Type, Func<ServiceProvider, object>>)value;

                var services = new List<KeyValuePair<Type, Func<ServiceProvider, object>>>();
                foreach (KeyValuePair<Type, Func<ServiceProvider, object>> item in items)
                {
                    services.Add(item);
                }

                // sort stuff
                services.Sort(delegate (KeyValuePair<Type, Func<ServiceProvider, object>> pair1, KeyValuePair<Type, Func<ServiceProvider, object>> pair2)
                {
                    Type serviceType1 = pair1.Key;
                    Type serviceType2 = pair2.Key;
                    string serviceType1Name = TypeUtil.GetFriendlyName(serviceType1, full.HasValue());
                    string serviceType2Name = TypeUtil.GetFriendlyName(serviceType2, full.HasValue());
                    return String.Compare(serviceType1Name, serviceType2Name, StringComparison.Ordinal);
                });


                var values = new List<Tuple<string, string>>();

                foreach (KeyValuePair<Type, Func<ServiceProvider, object>> service in services)
                {
                    Type serviceType = service.Key;
                    string serviceTypeName = TypeUtil.GetFriendlyName(serviceType, full.HasValue());

                    object serviceInstance = service.Value((ServiceProvider)serviceProvider);

                    bool isArray = false;

                    string serviceInstanceName = string.Empty;
                    if (serviceInstance != null)
                    {
                        isArray = serviceInstance.GetType().IsArray;

                        if (isArray)
                        {
                            Array arr = (Array)serviceInstance;
                            for (int i = 0; i < arr.Length; i++)
                            {
                                var arrItem = arr.GetValue(i);

                                if (arrItem != null)
                                {
                                    var tmp = TypeUtil.GetFriendlyName(arrItem.GetType(), full.HasValue());

                                    if (i == 0)
                                    {
                                        values.Add(new Tuple<string, string>(serviceTypeName, tmp));
                                    }
                                    else
                                    {
                                        values.Add(new Tuple<string, string>(" ...", tmp));
                                    }
                                }
                            }
                        }

                        if (!isArray)
                        {
                            serviceInstanceName = TypeUtil.GetFriendlyName(serviceInstance.GetType(), full.HasValue());
                        }
                    }

                    if (!isArray)
                    {
                        values.Add(new Tuple<string, string>(serviceTypeName, serviceInstanceName));
                    }
                }

                // write output
                context.Response.WriteInformation("List of all services available (type/implementation):");
                foreach (string text in OutputRenderer.FormatLines(values, true))
                {
                    context.Response.WriteInformation(text);
                }

                return Exit(context);
            }, context);
        }        
    }
}
