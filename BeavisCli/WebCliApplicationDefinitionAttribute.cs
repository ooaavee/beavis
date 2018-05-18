using System;
using System.Collections.Generic;
using System.Text;

namespace BeavisCli
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WebCliApplicationDefinitionAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
