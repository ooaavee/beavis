using System.Reflection;

namespace BeavisCli.Internal
{
    internal class DefaultWelcomeHandler : IWelcomeHandler
    {
        public void SayWelcome(ApplicationExecutionResponse response)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            AssemblyName name = assembly.GetName();
            AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            string version = $"{name.Version.Major}.{name.Version.Minor}.{name.Version.Build}";

            response.WriteSucceed($"{product.Product} {version}");
            response.WriteSucceed($"{copyright.Copyright}. MIT License.");
        }
    }
}
