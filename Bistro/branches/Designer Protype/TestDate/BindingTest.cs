using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Methods;

namespace Bistro.Tests
{
    public class BindingTest
    {
        public BindingTest(string bindingUrl, params BindingTest[] children)
        {
            this.bindingUrl = bindingUrl;
            foreach (BindingTest child in children)
                this.children.Add(child.bindingUrl, child);
            controllers = new ControllerTest[] { };
        }

        public BindingTest(string bindingUrl, ControllerTest[] controllers, params BindingTest[] children)
        {
            this.bindingUrl = bindingUrl;
            foreach (BindingTest child in children)
                this.children.Add(child.bindingUrl, child);
            this.controllers = controllers;
        }
        string bindingUrl;
        Dictionary<string, BindingTest> children = new Dictionary<string,BindingTest>();
        ControllerTest[] controllers;

        internal void Validate(Binding binding)
        {
            Validate("", binding);
        }

        internal void Validate(string parentUrl, Binding binding)
        {
            string fullUrl = parentUrl + bindingUrl;
            binding.Bindings.ForEach(child =>
                {
                    BindingTest test = null;
                    string lookup = (child.BindingUrl == child.FullBindingUrl) ? (child.Verb + " " + child.BindingUrl) : child.BindingUrl;

                    test.Validate(fullUrl, child);
                });
            
            SortedList<int, Controller> controllers = new SortedList<int,Controller>();
            binding.Controllers.ForEach(controller => controllers.Add(controller.SeqNumber, controller));

            for (int i = 0; i < this.controllers.Length && i < binding.Controllers.Count; i++)
                this.controllers[i].Validate(fullUrl, controllers.Values[binding.Controllers.Count - i - 1]);
        }

    }

}
