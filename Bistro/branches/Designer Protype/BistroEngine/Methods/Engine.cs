using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Bistro.Controllers.Descriptor;
using System.Text.RegularExpressions;
using Bistro.Methods.Reflection;
using Bistro.Controllers.Descriptor;

namespace Bistro.Methods
{
    public class Engine
    {
        Binding root;

        public Engine()
        {
            root = new Binding(this);
        }

        public void ProcessControllers(List<string> removed, List<ITypeInfo> controllers)
        {
            ControllerType type;
            foreach (string name in removed)
                if (controllerTypes.TryGetValue(name, out type))
                {
                    controllerTypes.Remove(name);
                    type.Dispose();
                }
            controllers.ForEach(controller => CreateBindingNodes(controller));
            root.Validate();
        }

        internal Binding Root { get { return root; } }

//        Regex bindingParser = new Regex("^\\s*(?'binding'(\\w*\\s*)(\\?|/|(/(\\*|(\\w|-)+|\\{\\w+}|\\?/((\\w|-)+|\\{\\w+})))*(/\\?)?))\\s*$", RegexOptions.Compiled | RegexOptions.Singleline);
        Regex bindingParser = new Regex(@"^\s*(?'binding'(\w*\s*)((\?|/|(/(\*|(\w|-)+|\{\w+}|\?/((\w|-)+|\{\w+})))*(/\?)?)(?:\?(?:(?:\w|-|=)+|\{\w+})(?:\&(?:(?:\w|-|=)+|\{\w+}))*|)))\s*$", RegexOptions.Compiled | RegexOptions.Singleline);

        Dictionary<string, ControllerType> controllerTypes = new Dictionary<string, ControllerType>();

        internal virtual void RaiseInvalidBinding(ControllerType controller, params string[] args)
        {
        }

        internal virtual void RaiseResourceLoop(string methodUrl, IEnumerable<Controller> controllers, params string[] args)
        {
        }

        internal virtual void RaiseMissingProvider(string methodUrl, string resName, IEnumerable<ControllerType> controllers, params string[] args)
        {
        }

        internal virtual void RaiseInconsistentResourceType(string methodUrl, string resName, IEnumerable<ControllerType> controllers, params string[] args)
        {
        }

        private void CreateBindingNodes(ITypeInfo classInfo)
        {

            ControllerType type;
            if (controllerTypes.TryGetValue(classInfo.FullName, out type))
            {
                controllerTypes.Remove(classInfo.FullName);
                type.Dispose();
            }
            type = new ControllerType(classInfo);

            controllerTypes.Add(type.Name, type);

            List<string> bindings = new List<string>();
            foreach (IAttributeInfo attribute in classInfo.Attributes)
                if (attribute.Type == typeof(BindAttribute).FullName && attribute.Parameters.Count > 0)
                    bindings.Add(attribute.Parameters[0].AsString());
            if (bindings.Count == 0)
            {
                // TODO: define the default binding
            }

            foreach (string item in bindings)
            {
                Match match = bindingParser.Match(item);
                if (match.Success)
                {
                    string methodUrl = match.Groups["binding"].Captures[0].Value;
                    string verb;
                    switch (methodUrl[0])
                    {
                        case '?':
                        case '/':
                            verb = "*";
                            break;
                        default:
                            verb = methodUrl.Substring(0, methodUrl.IndexOfAny(new char[] { ' ', '/', '?' }));
                            methodUrl = methodUrl.Substring(methodUrl.IndexOfAny(new char[] { '/', '?' }));
                            break;

                    }
                    foreach (Binding method in root.PlaceController(verb, methodUrl))
                        //                        method.Register(new Controller(method, type, methodUrl));
                        method.Register(method, type, methodUrl);
                }
                else
                    RaiseInvalidBinding(type, item);
            }
        }

    }
}
