﻿using System;
using BeavisCli.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBeavis(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<ApplicationSpace>();
            services.AddSingleton<RequestHandler>();
            services.AddSingleton<WebTerminalRenderer>();

            services.AddSingleton<Kanta, K1>();
            services.AddSingleton<Kanta, K2>();

            return services;
        }

    }

    public abstract class Kanta
    {

    }

    public class K1 : Kanta
    {

    }


    public class K2 : Kanta
    {

    }
}
