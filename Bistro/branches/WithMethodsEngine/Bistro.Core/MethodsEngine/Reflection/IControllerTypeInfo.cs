using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers.Descriptor;

namespace Bistro.MethodsEngine.Reflection
{
    public interface IControllerTypeInfo
    {
        string ControllerTypeName { get; }

        List<string> DependsOn { get; }

        List<string> Requires { get; }

        List<string> Provides { get; }

        Dictionary<string, IMemberInfo> FormFieldsList { get; }

        Dictionary<string, IMemberInfo> RequestFieldsList { get; }

        Dictionary<string, IMemberInfo> SessionFieldsList { get; }

        Dictionary<string, IMemberInfo> CookieFieldsList { get; }

        bool IsSecurity { get; }

        string GetResourceType(string resourceName);

    }
}
