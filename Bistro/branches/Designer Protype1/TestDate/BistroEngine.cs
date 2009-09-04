using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Methods;
using Bistro.Methods.Reflection;

namespace TestDate
{

    public class BistroEngine : Engine
    {
        public class ControllerEventArgs : EventArgs
        {
            internal ControllerEventArgs(ITypeInfo controller, params string[] args) { Controller = controller; Args = args; }
            public ITypeInfo Controller { get; private set; }
            public string[] Args { get; private set; }
        }

        public delegate void ControllerEvent(object sender, ControllerEventArgs e);

        public event ControllerEvent OnInvalidBinding;

        public class MethodEventArgs : EventArgs
        {
            internal MethodEventArgs(string methodUrl, string resourceName, IEnumerable<ITypeInfo> controllers, params string[] args)
            { MethodUrl = methodUrl; ResourceName = resourceName; Controllers = controllers; Args = args; }
            public string MethodUrl { get; private set; }
            public string ResourceName { get; private set; }
            public IEnumerable<ITypeInfo> Controllers { get; private set; }
            public string[] Args { get; private set; }
        }

        public class MethodResourceEventArgs : EventArgs
        {
            internal MethodResourceEventArgs(string methodUrl, string resourceName, IEnumerable<ControllerType> controllers, params string[] args)
            { MethodUrl = methodUrl; ResourceName = resourceName; Controllers = controllers; Args = args; }
            public string MethodUrl { get; private set; }
            public string ResourceName { get; private set; }
            public IEnumerable<ControllerType> Controllers { get; private set; }
            public string[] Args { get; private set; }
        }



        class controllerIterator : IEnumerable<ITypeInfo>
        {
            public controllerIterator(IEnumerable<Controller> controllers)
            {
                this.controllers = controllers;
            }
            IEnumerable<Controller> controllers;

            #region IEnumerable<ITypeInfo> Members

            public IEnumerator<ITypeInfo> GetEnumerator()
            {
                foreach (Controller c in controllers)
                    yield return c.Type.Type;
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        class controllerTypeIterator : IEnumerable<ITypeInfo>
        {
            public controllerTypeIterator(IEnumerable<ControllerType> controllers)
            {
                this.controllers = controllers;
            }
            IEnumerable<ControllerType> controllers;

            #region IEnumerable<ITypeInfo> Members

            public IEnumerator<ITypeInfo> GetEnumerator()
            {
                foreach (ControllerType c in controllers)
                    yield return c.Type;
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        public delegate void MethodEvent(object sender, MethodEventArgs e);

        public delegate void MethodResourceEvent(object sender, MethodResourceEventArgs e);

        public event MethodEvent OnResourceLoop;

        public override void RaiseInvalidBinding(ControllerType controller, params string[] args)
        {
            if (OnInvalidBinding != null)
                OnInvalidBinding(this, new ControllerEventArgs(controller.Type, args));
        }

        public override void RaiseResourceLoop(string methodUrl, IEnumerable<Controller> controllers, params string[] args)
        {
            if (OnResourceLoop != null)
                OnResourceLoop(this, new MethodEventArgs(methodUrl, null, new controllerIterator(controllers), args));
        }

        public event MethodEvent OnMissingProvider;

        public override void RaiseMissingProvider(string methodUrl, string resName, IEnumerable<ControllerType> controllers, params string[] args)
        {
            if (OnMissingProvider != null)
                OnMissingProvider(this, new MethodEventArgs(methodUrl, resName, new controllerTypeIterator(controllers), args));
        }

        public event MethodResourceEvent OnInconsistentResourceType;

        public override void RaiseInconsistentResourceType(string methodUrl, string resName, IEnumerable<ControllerType> controllers, params string[] args)
        {
            if (OnInconsistentResourceType != null)
                OnInconsistentResourceType(this, new MethodResourceEventArgs(methodUrl, resName, controllers, args));
        }

    }
}
