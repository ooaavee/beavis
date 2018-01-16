using System;
using System.Collections.Generic;
using System.Text;

namespace BeavisCli
{
    /// <summary>
    /// Information message
    /// </summary>
    public class SuccessMessage : ResponseMessage
    {
        public override string Type => "success";
    }
}
