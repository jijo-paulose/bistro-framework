using System;
using System.Collections.Generic;
using System.Text;
using BistroApi;

namespace BistroModel
{
    /// <summary>
    /// Marks a class as bound to a particular meta-url
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class BindAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindAttribute"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public BindAttribute(string target)
        {
            this.target = target;
        }

        /// <summary>
        /// The target url
        /// </summary>
        private string target;

        /// <summary>
        /// Gets or sets the target url structure
        /// </summary>
        /// <value>The target.</value>
        public string Target
        {
            get { return target; }
            set { target = value; }
        }

        /// <summary>
        /// The priority of this bind point
        /// </summary>
        private int priority = -1;

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        /// <summary>
        /// The bind type of the controller.
        /// </summary>
        private BindType controllerBindType = BindType.Before;

        /// <summary>
        /// Determines when the controller will be executed in the request execution pipeline
        /// </summary>
        /// <value>The type of the controller bind.</value>
        public BindType ControllerBindType
        {
            get { return controllerBindType; }
            set { controllerBindType = value; }
        }
    }
}
