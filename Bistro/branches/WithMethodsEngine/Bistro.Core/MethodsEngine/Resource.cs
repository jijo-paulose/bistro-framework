using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.MethodsEngine.Reflection;

namespace Bistro.MethodsEngine
{
    internal class Resource
    {
        internal Resource(Engine engine, string name)
        {
            this.name = name;
            this.engine = engine;
        }
        Engine engine;

        private string name;
        private List<IBindPointDescriptor> providers = new List<IBindPointDescriptor>();
        private List<IBindPointDescriptor> dependents = new List<IBindPointDescriptor>();
        private List<IBindPointDescriptor> requiredBy = new List<IBindPointDescriptor>();

        public string Name { get { return name; } }
        public IEnumerable<IBindPointDescriptor> Providers { get { return providers; } }
        public IEnumerable<IBindPointDescriptor> Dependents { get { return dependents; } }
        public IEnumerable<IBindPointDescriptor> RequiredBy { get { return requiredBy; } }
        public bool IsEmpty { get { return providers.Count + dependents.Count + requiredBy.Count == 0; } }

        internal void AddProvider(IBindPointDescriptor bindPoint)
        {
            providers.Add(bindPoint);
        }

        internal void AddDependents(IBindPointDescriptor bindPoint)
        {
            dependents.Add(bindPoint);
        }

        internal void AddRequiredBy(IBindPointDescriptor bindPoint)
        {
            requiredBy.Add(bindPoint);
        }

        internal void Validate()
        {
            if (providers.Count == 0 && requiredBy.Count > 0)
                engine.RaiseMissingProvider(this.name, dependents.Concat(requiredBy).Select((bpd) => bpd.ControllerInfo));
            string resourceType = null;

            resourceType = ValidateResourceType(Providers, resourceType);
            resourceType = ValidateResourceType(Dependents, resourceType);
            resourceType = ValidateResourceType(RequiredBy, resourceType);
        }

        private string ValidateResourceType(IEnumerable<IBindPointDescriptor> bindPoints, string type)
        {
            foreach (IBindPointDescriptor bindPoint in bindPoints)
                if (type == null)
                    type = bindPoint.ControllerInfo.GetResourceType(name);
                else
                    if (type != bindPoint.ControllerInfo.GetResourceType(name))
                        engine.RaiseInconsistentResourceType(bindPoint.Target, this.name, providers.Concat(dependents).Concat(requiredBy).Select((bpd) => bpd.ControllerInfo));
            return type;
        }


        internal void Unregister(IBindPointDescriptor bindPoint)
        {
            providers.RemoveAll(provider => provider == bindPoint);
            dependents.RemoveAll(dependent => dependent == bindPoint);
            requiredBy.RemoveAll(reqBy => reqBy == bindPoint);
        }



    }
}
