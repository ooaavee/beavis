using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultWebCliInitializer : IWebCliInitializer
    {
        public void Initialize(HttpContext context, WebCliResponse response)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            AssemblyName name = assembly.GetName();

            AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();

            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();

            string line1 = $"{product.Product} {name.Version.Major}.{name.Version.Minor}.{name.Version.Build}";
            string line2 = $"{copyright.Copyright}. Code released under the MIT License. Usage 'license' for more details.";

            response.WriteSuccess(line1);
            response.WriteSuccess(line2);
        }
    }
}
