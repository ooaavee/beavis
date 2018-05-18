using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BeavisCli.Debugging.Applications
{
    [WebCliApplicationDefinition(Name = "services", Description = "Lists all services available.")]
    public class Services : WebCliApplication
    { 
        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {
                // get all services
                IServiceProvider serviceProvider = context.HttpContext.RequestServices;
                Type type = serviceProvider.GetType();
                PropertyInfo property = type.GetProperty("RealizedServices", BindingFlags.Instance | BindingFlags.NonPublic);
                var value = property.GetValue(serviceProvider);
                ConcurrentDictionary<Type, Func<ServiceProvider, object>> services = (ConcurrentDictionary<Type, Func<ServiceProvider, object>>)value;

                // resolve service and implementation names
                var values = new List<Tuple<string, string>>();
                foreach (var service in services)
                {
                    Type serviceType = service.Key;
                    string serviceTypeName = GetFriendlyName(serviceType);

                    object serviceInstance = service.Value((ServiceProvider)serviceProvider); 

                    // TODO: Jos serviceInstance on array, niin iteroi taulukon jäsenet lävitse ja tulosta niiden tyypit!

                    string serviceInstanceName = string.Empty;
                    if (serviceInstance != null)
                    {
                        serviceInstanceName = GetFriendlyName(serviceInstance.GetType());
                    }

                    values.Add(new Tuple<string, string>(serviceTypeName, serviceInstanceName));
                }

                // sort stuff
                values.Sort((tuple1, tuple2) => String.Compare(tuple1.Item1, tuple2.Item1, StringComparison.Ordinal));

                // write output
                context.Response.WriteInformation("List of all services available (type/implementation):");
                foreach (string text in OutputRenderer.FormatLines(values, true))
                {
                    context.Response.WriteInformation(text);
                }

                return Exit(context);
            }, context);
        }

        private static readonly Dictionary<Type, string> TypeNames = new Dictionary<Type, string>
        {
            {typeof(int), "int"},
            {typeof(uint), "uint"},
            {typeof(long), "long"},
            {typeof(ulong), "ulong"},
            {typeof(short), "short"},
            {typeof(ushort), "ushort"},
            {typeof(byte), "byte"},
            {typeof(sbyte), "sbyte"},
            {typeof(bool), "bool"},
            {typeof(float), "float"},
            {typeof(double), "double"},
            {typeof(decimal), "decimal"},
            {typeof(char), "char"},
            {typeof(string), "string"},
            {typeof(object), "object"},
            {typeof(void), "void"}
        };

        private static string GetFriendlyName(Type type, Dictionary<Type, string> translations)
        {
            if (translations.ContainsKey(type))
            {
                return translations[type];
            }

            if (type.IsArray)
            {
                return GetFriendlyName(type.GetElementType(), translations) + "[]";
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return GetFriendlyName(type.GetGenericArguments()[0]) + "?";
            }

            if (type.IsGenericType)
            {
                return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName).ToArray()) + ">";
            }

            return type.Name;
        }

        private static string GetFriendlyName(Type type)
        {
            return GetFriendlyName(type, TypeNames);
        }
    }
}
