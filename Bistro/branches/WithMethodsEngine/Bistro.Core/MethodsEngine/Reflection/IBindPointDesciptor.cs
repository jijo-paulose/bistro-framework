using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers.Descriptor;

namespace Bistro.MethodsEngine.Reflection
{
    public interface IBindPointDescriptor
    {
        BindType ControllerBindType { get; }
        string Target { get; }
        int Priority { get; }
        IControllerTypeInfo ControllerInfo { get; }
    }
}
