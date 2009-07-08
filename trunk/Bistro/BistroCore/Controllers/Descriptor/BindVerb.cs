using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Controllers.Descriptor
{
    /// <summary>
    /// Enum of valid bind terms
    /// </summary>
    [Flags]
    public enum BindVerb
    {
        GET = 1,
        POST = 2,
        PUT = 4,
        HEAD = 8,
        DELETE = 16
    }
}
