using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli
{
    public sealed class ApplicationInfo
    {
        /////// <summary>
        /////// All registered applications.
        /////// </summary>
        ////public static List<ApplicationInfo> RegisteredApplications { get; } = new List<ApplicationInfo>();

        /////// <summary>
        /////// Application type
        /////// </summary>
        ////public Type Type { get; set; }

        /// <summary>
        /// Application name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Application description
        /// </summary>
        public string Description { get; set; }




        /////// <summary>
        /////// Allow application execution without authentication?
        /////// </summary>
        ////public bool AllowAnonymous { get; set; }

        /////// <summary>
        /////// Creates an application instance by it's name.
        /////// </summary>
        /////// <param name="name">application name</param>
        /////// <returns>an application instance or null if not found</returns>
        ////public static AbstractBeavisApplication CreateApplicationInstance(string name, CliContext context)
        ////{
        ////    throw new NotImplementedException();
        ////    ////AbstractApplication app = null;
        ////    ////ApplicationInfo info = RegisteredApplications.FirstOrDefault(x => x.Name == name);

        ////    ////if (info != null)
        ////    ////{
        ////    ////    if (IsAvailable(info, context))
        ////    ////    {
        ////    ////        app = (AbstractApplication)Activator.CreateInstance(info.Type);
        ////    ////        app.GetInfo = () => info;
        ////    ////    }
        ////    ////}

        ////    ////return app;
        ////}

        ////public static bool IsAvailable(ApplicationInfo info, CliContext context)
        ////{
            
        ////    return true;
        ////}

    }
}
