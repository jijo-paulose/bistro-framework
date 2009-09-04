using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Methods
{
    internal class Resource
    {
        internal Resource(Engine engine, Binding binding, string name)
        {
            this.name = name;
            this.engine = engine;
            this.binding = binding;
        }
        Engine engine;
        Binding binding;

        string name;
        List<ControllerType> providers = new List<ControllerType>();
        List<ControllerType> dependents = new List<ControllerType>();
        List<ControllerType> requiredBy = new List<ControllerType>();

        public string Name { get { return name; } }
        public IEnumerable<ControllerType> Providers { get { return providers; } }
        public IEnumerable<ControllerType> Dependents { get { return dependents; } }
        public IEnumerable<ControllerType> RequiredBy { get { return requiredBy; } }
        public bool IsEmpty { get { return providers.Count + dependents.Count + requiredBy.Count == 0; } }

        internal void AddProvider(ControllerType controller)
        {
            providers.Add(controller);
            controller.Register(this);
        }

        internal void AddDependents(ControllerType controller)
        {
            dependents.Add(controller);
            controller.Register(this);
        }

        internal void AddRequiredBy(ControllerType controller)
        {
            requiredBy.Add(controller);
            controller.Register(this);
        }

        internal void Validate()
        {
            if (providers.Count == 0 && requiredBy.Count > 0)
                engine.RaiseMissingProvider(binding.FullBindingUrl,this.name, dependents.Concat(requiredBy), name);
            string resourceType = null;

            resourceType = ValidateResourceType(Providers, resourceType);
            resourceType = ValidateResourceType(Dependents, resourceType);
            resourceType = ValidateResourceType(RequiredBy, resourceType);
        }

        private string ValidateResourceType(IEnumerable<ControllerType> controllers, string type)
        {
            foreach (ControllerType controller in controllers)
                if (type == null)
                    type = controller.GetResourceType(name);
                else
                    if (type != controller.GetResourceType(name))
                        engine.RaiseInconsistentResourceType(binding.FullBindingUrl, this.name, providers.Concat(dependents).Concat(requiredBy));
            return type;
        }


        internal void Unregister(ControllerType controllerType)
        {
            providers.RemoveAll(provider => provider == controllerType);
            dependents.RemoveAll(provider => provider == controllerType);
            requiredBy.RemoveAll(provider => provider == controllerType);
        }
    }
}
