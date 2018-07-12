using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace BeavisCli
{
    public class DefaultWebCliInitializer : IWebCliInitializer
    {
        public virtual void Initialize(HttpContext context, WebCliResponse response)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            AssemblyName name = assembly.GetName();
            AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            string version = $"{name.Version.Major}.{name.Version.Minor}.{name.Version.Build}";

            response.WriteSuccess($"{product.Product} {version}");
            response.WriteSuccess($"{copyright.Copyright}. MIT License.");
        }
    }
}
