/****************************************************************************
 * 
 *  Bistro Framework Copyright © 2003-2009 Hill30 Inc
 *
 *  This file is part of Bistro Framework.
 *
 *  Bistro Framework is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Bistro Framework is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Bistro Framework.  If not, see <http://www.gnu.org/licenses/>.
 *  
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.MethodsEngine.Reflection;
using Bistro.Controllers.Descriptor;

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
        private List<IMethodsBindPointDesc> providers = new List<IMethodsBindPointDesc>();
        private List<IMethodsBindPointDesc> dependents = new List<IMethodsBindPointDesc>();
        private List<IMethodsBindPointDesc> requiredBy = new List<IMethodsBindPointDesc>();

        public string Name { get { return name; } }
        public IEnumerable<IMethodsBindPointDesc> Providers { get { return providers; } }
        public IEnumerable<IMethodsBindPointDesc> Dependents { get { return dependents; } }
        public IEnumerable<IMethodsBindPointDesc> RequiredBy { get { return requiredBy; } }
        public bool IsEmpty { get { return providers.Count + dependents.Count + requiredBy.Count == 0; } }

        internal void AddProvider(IMethodsBindPointDesc bindPoint)
        {
            providers.Add(bindPoint);
        }

        internal void AddDependents(IMethodsBindPointDesc bindPoint)
        {
            dependents.Add(bindPoint);
        }

        internal void AddRequiredBy(IMethodsBindPointDesc bindPoint)
        {
            requiredBy.Add(bindPoint);
        }

        internal void Validate()
        {
            if (providers.Count == 0 && requiredBy.Count > 0)
                engine.RaiseMissingProvider(this.name, dependents.Concat(requiredBy).Select((bpd) => bpd.ControllerMethodDesc));
            string resourceType = null;

            resourceType = ValidateResourceType(Providers, resourceType);
            resourceType = ValidateResourceType(Dependents, resourceType);
            resourceType = ValidateResourceType(RequiredBy, resourceType);
        }

        private string ValidateResourceType(IEnumerable<IMethodsBindPointDesc> bindPoints, string type)
        {
            foreach (IMethodsBindPointDesc bindPoint in bindPoints)
                if (type == null)
                    type = bindPoint.ControllerMethodDesc.GetResourceType(name);
                else
                    if (type != bindPoint.ControllerMethodDesc.GetResourceType(name))
                        engine.RaiseInconsistentResourceType(bindPoint.Target, this.name, providers.Concat(dependents).Concat(requiredBy).Select((bpd) => bpd.ControllerMethodDesc));
            return type;
        }


        internal void Unregister(IMethodsBindPointDesc bindPoint)
        {
            providers.RemoveAll(provider => provider == bindPoint);
            dependents.RemoveAll(dependent => dependent == bindPoint);
            requiredBy.RemoveAll(reqBy => reqBy == bindPoint);
        }



    }
}
