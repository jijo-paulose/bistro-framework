using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bistro.Methods;

namespace Bistro.Tests
{
    internal class BindingTest
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

        public void Validate(Binding binding)
        {
            Validate("", binding);
        }

        public void Validate(string parentUrl, Binding binding)
        {
            string fullUrl = parentUrl + bindingUrl;
            binding.Bindings.ForEach(child =>
                {
                    BindingTest test;
                    string lookup = (child.BindingUrl == child.FullBindingUrl) ? (child.Verb + " " + child.BindingUrl) : child.BindingUrl;

                    Assert.IsTrue(children.TryGetValue(lookup, out test), "Binding " + fullUrl + " Unexpected binding " + child.BindingUrl);
                    test.Validate(fullUrl, child);
                });
            Assert.AreEqual(children.Count, binding.Bindings.Count, "Binding " + fullUrl + ": Invalid number of child bindings");
            
            SortedList<int, Controller> controllers = new SortedList<int,Controller>();
            binding.Controllers.ForEach(controller => controllers.Add(controller.SeqNumber, controller));

            for (int i = 0; i < this.controllers.Length && i < binding.Controllers.Count; i++)
                this.controllers[i].Validate(fullUrl, controllers.Values[binding.Controllers.Count - i - 1]);
            Assert.AreEqual(this.controllers.Length, binding.Controllers.Count, "Binding " + fullUrl + ": Invalid number of controllers");
        }

    }

}
