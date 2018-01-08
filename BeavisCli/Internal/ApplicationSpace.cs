using System;
using System.Collections.Generic;
using System.Linq;

namespace BeavisCli.Internal
{
    internal class ApplicationSpace
    {
        private readonly AbstractApplication[] _applications;

        public ApplicationSpace(IEnumerable<AbstractApplication> applications)
        {
            _applications = applications.ToArray();
        }

        public AbstractApplication FindApplicaton(string name)
        {
            var found = new List<AbstractApplication>();

            foreach (var app in _applications)
            {
                var info = app.GetInfo();
                if (info.Name == name)
                {
                    found.Add(app);
                }
            }

            if (!found.Any())
            {
                throw new ApplicationSpaceException($"{name} is not a valid application.{Environment.NewLine}Usage 'help' to get list of applications.");
            }

            if (found.Count > 1)
            {
                throw new ApplicationSpaceException($"Found more than one application with name '{name}'. Application names must me unique.");
            }

            return found.Single();
        }
    }
}