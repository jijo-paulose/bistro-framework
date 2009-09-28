using System.Collections.Generic;

using System.Text.RegularExpressions;
using Bistro.Methods.Reflection;
//using Bistro.Controllers.Descriptor.Data;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;

namespace Bistro.Methods
{
	public class ControllerType
	{
		public ControllerType(ITypeInfo type)
		{
			this.type = type;

            foreach (IAttributeInfo attribute in type.Attributes)
                if (attribute.Type == typeof(BindAttribute).FullName && attribute.Parameters.Count > 0)
                    bindings.Add(attribute.Parameters[0].AsString());

            foreach (IFieldInfo field in type.Fields)
                registerResources(field.Name, field.Attributes, field.Type);

            foreach (IPropertyInfo property in type.Properties)
                registerResources(property.Name, property.Attributes, property.Type);
        }

		Dictionary<string, string> resourceTypes = new Dictionary<string, string>();
		private void registerResources(string resourceName, IEnumerable<IAttributeInfo> attributes, string resourceType)
		{
			bool consumesResource = false;
			bool isProvider = false;
			bool maybeProvider = false;
			foreach (IAttributeInfo attribute in attributes)
			{
				if (attribute.Type == typeof(ProvidesAttribute).FullName)
				{
					isProvider = true;
					if (attribute.Parameters.Count > 0)
						provides.Add(attribute.Parameters[0].AsString());
					else
						provides.Add(resourceName);
					resourceTypes[resourceName] = resourceType;
				}

				if (attribute.Type == typeof(SessionAttribute).FullName || attribute.Type == typeof(RequestAttribute).FullName)
					maybeProvider = true;

				if (attribute.Type == typeof(DependsOnAttribute).FullName)
				{
					consumesResource = true;
					if (attribute.Parameters.Count > 0)
						dependsOn.Add(attribute.Parameters[0].AsString());
					else
						dependsOn.Add(resourceName);
					resourceTypes[resourceName] = resourceType;
				}

				if (attribute.Type == typeof(RequiresAttribute).FullName)
				{
					consumesResource = true;
					if (attribute.Parameters.Count > 0)
						requires.Add(attribute.Parameters[0].AsString());
					else
						requires.Add(resourceName);
					resourceTypes[resourceName] = resourceType;
				}
			}

			if (maybeProvider && !isProvider && !consumesResource)
			{
				provides.Add(resourceName);
				resourceTypes[resourceName] = resourceType;
			}

		}

		ITypeInfo type;
		List<string> bindings = new List<string>();
		List<string> provides = new List<string>();
		List<string> dependsOn = new List<string>();
		List<string> requires = new List<string>();

		public string Name { get { return type.FullName; } }

		public IEnumerable<string> Provides { get { return provides; } }

		public IEnumerable<string> DependsOn { get { return dependsOn; } }

		public IEnumerable<string> Requires { get { return requires; } }

		public ITypeInfo Type { get { return type; } }

		List<Controller> controllers = new List<Controller>();

		internal void Register(Controller controller)
		{
			controllers.Add(controller);
		}

		internal void Unregister(Controller controller)
		{
			controllers.Remove(controller);
		}

        List<Resource> resources = new List<Resource>();

        internal void Register(Resource resource)
        {
            if (!resources.Contains(resource))
                resources.Add(resource);
        }

        internal void Unregister(Resource resource)
        {
            resources.Remove(resource);
        }

        internal string GetResourceType(string Name)
        {
            return resourceTypes[Name];
        }

        public void Dispose()
        {
            controllers.ForEach(controller => controller.Dispose());
            resources.ForEach(resource => resource.Unregister(this));
        }
    }
}
