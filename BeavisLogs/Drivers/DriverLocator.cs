using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisLogs.Drivers
{
    public static class DriverLocator
    {
        /// <summary>
        /// Finds the driver by driver type.
        /// </summary>
        /// <param name="driverType"><see cref="IDriver"/> implementation type name</param>
        /// <param name="context">HTTP context</param>
        /// <returns>driver implementation or null if not found</returns>
        public static IDriver FindDriver(string driverType, HttpContext context)
        {
            foreach (IDriver driver in context.RequestServices.GetServices<IDriver>())
            {
                string tmp = driver.GetType().FullName;

                if (driverType == tmp)
                {
                    return driver;
                }
            }

            return null;
        }
    }
}
