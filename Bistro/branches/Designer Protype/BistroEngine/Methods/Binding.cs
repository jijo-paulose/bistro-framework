using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Methods.Reflection;

namespace Bistro.Methods
{
    public class Binding
    {
        Binding parent;
        Engine engine;
        string verb;
        string bindingUrl;
        string fullBindingUrl;
        bool isMethod = false;

        public Binding(Engine engine)
        {
            this.engine = engine;
            verb = "*";
            fullBindingUrl = "";
            bindingUrl = "";
        }

        private Binding(Binding parent, string verb, string bindingUrl)
        {
            this.parent = parent;
            engine = parent.engine;
            this.verb = verb;
            this.bindingUrl = bindingUrl;
            fullBindingUrl = parent.fullBindingUrl + bindingUrl;
            parent.bindings.Add(this);
        }

        private void Adopt(Binding binding, string bindingUrl)
        {
            binding.parent.bindings.Remove(binding);
            binding.parent = this;
            bindings.Add(binding);
            binding.bindingUrl = bindingUrl;
        }

        private Dictionary<string, Binding> partialUrls = new Dictionary<string, Binding>();
        private List<Binding> bindings = new List<Binding>();
        private List<Controller> controllers = new List<Controller>();
        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

        public Dictionary<string, Resource> Resources { get { return resources; } }
        public List<Binding> Bindings { get { return bindings; } }
        public string Verb { get { return verb; } }
        public string BindingUrl { get { return bindingUrl; } }
        public string FullBindingUrl { get { return fullBindingUrl; } }
        public List<Controller> Controllers { get { return controllers; } }

        private Binding CreateBinding(string verb, string bindingUrl)
        {
            return new Binding(this, verb, bindingUrl);
        }

        /// <summary>
        /// Locates or creates a list of <see cref="T:Binding">Binding Nodes</see> to bind the controller to
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="controller"></param>
        /// <returns>A list of <see cref="T:Binding"/> objects the controller has to be bound to</returns>
        /// <remarks> The <see cref="Binding"/> objects are created by this method as necessary </remarks>
        internal List<Binding> PlaceController(string verb, string bindingUrl)
        {
            List<Binding> result = new List<Binding>();

            Binding binding = null;

            // look up a child node with binding at least partially matching
            // that of the supplied controller. If none found, create it
            string path = bindingUrl;
            while (!partialUrls.TryGetValue(verb + ',' + path, out binding))
            {
                int separatorPos = path.LastIndexOf('/');
                if (separatorPos <= 0)
                {
                    path = bindingUrl;
                    break;
                }
                path = path.Substring(0, separatorPos);
            }

            if (binding == null)
            {
                // we did not find a node matching our method - let us create
                // a new binding node representing a new method
                binding = CreateBinding(verb, bindingUrl);
                RegisterBindings(binding, null);
            }

            if (binding.bindingUrl != path)
            {
                // the binding we found is not really our method node, but rather 
                // a parent of the (missing) method node. Let us create the real 
                // method node
                Binding realNode = CreateBinding(verb, path);
                RegisterBindings(realNode, binding);
                realNode.Adopt(binding, binding.bindingUrl.Substring(path.Length));
                realNode.RegisterBindings(binding, null);
                binding = realNode;
            }

            if (path == bindingUrl)
            {
                // at this point we know that 'this' points to the method binding
                if (!binding.isMethod)
                {
                    // This is not a method binding yet, let us it turn it into one
                    binding.isMethod = true;

                    // Clone controllers from all other methods as implied by the 
                    // wild cards in this method binding
                    engine.Root.MatchBindingsToMethod(binding);
                }
                // Build a list of all other method nodes this controller has to be added to 
                // because of the wild cards in other method urls 
                result.AddRange(engine.Root.MatchMethodsToBinding(binding));
            }
            else
                // 'this' is not the method node - the 'methodNode' is. call PlaceController
                // recursively
                result.AddRange(binding.PlaceController(verb, bindingUrl.Substring(path.Length)));

            return result;
        }

        private void RegisterBindings(Binding newBinding, Binding oldBinding)
        {
            foreach (KeyValuePair<string, Binding> binding in new List<KeyValuePair<string, Binding>>(partialUrls))
                if (binding.Value == oldBinding)
                    partialUrls.Remove(binding.Key);
            if (newBinding == null)
                return;

            string bindingUrl = newBinding.bindingUrl;
            while (true)
            {
                partialUrls.Add(newBinding.verb + ',' + bindingUrl, newBinding);
                if (bindingUrl.LastIndexOf('/') < 1)
                    break;
                bindingUrl = bindingUrl.Substring(0, bindingUrl.LastIndexOf('/'));
            }
        }

        /// <summary>
        /// Matches the new node to all existing methods. If a match found clone
        /// controllers from the matching method to the new method
        /// </summary>
        /// <param name="methodNode"></param>
        /// <remarks>
        /// this method is called for every method node when the node is created. <br/>
        /// It recursively traverses the node tree looking if any existing method node
        /// matches the new method node. It can only happen if the existing node
        /// has a wild card url, otherwise our method node would already have been created
        /// </remarks>
        private void MatchBindingsToMethod(Binding methodNode)
        {
            foreach (Binding child in bindings)
            {
                child.MatchBindingsToMethod(methodNode);
                // this is our node - we do not need to do it again
                if (child == methodNode)
                    continue;
                // this is not a method - skip it
                if (!child.isMethod)
                    continue;
                if (Match(methodNode, child))
                    // it is a match - all controllers from this method
                    // have to be cloned to our new method
                    foreach (Controller controller in child.controllers)
                        //                        methodNode.Register(new Controller(methodNode, controller));
                        methodNode.Register(methodNode, controller.Type, null);
            }
        }

        /// <summary>
        /// Matches the new binding to all methods
        /// </summary>
        /// <param name="binding"></param>
        /// <returns>a list of all <see cref="BindingNode"/> matching the url </returns>
        private IEnumerable<Binding> MatchMethodsToBinding(Binding binding)
        {
            List<Binding> result = new List<Binding>();
            foreach (Binding child in bindings)
            {
                result.AddRange(child.MatchMethodsToBinding(binding));

                if (child.isMethod && Match(child, binding))
                    result.Add(child);
            }
            return result;
        }

        /// <summary>
        /// Matches method to binding
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        /// <remarks>method binding is always treated literally, while binding binding is
        /// checked for wildcards</remarks>
        private static bool Match(Binding method, Binding binding)
        {
            string verb = method.verb;
            string methodUrl = method.fullBindingUrl;
            string bindingVerb = binding.verb;
            string bindingUrl = binding.fullBindingUrl;

            if (verb != bindingVerb && bindingVerb != "*")
                return false;

            string[] MethodTokens = methodUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] BindingTokens = bindingUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int i, j;
            for (i = 0, j = 0; i < BindingTokens.Length && j < MethodTokens.Length; i++, j++)
            {
                if (BindingTokens[i] == MethodTokens[j])
                    continue; // so far so good - keep going
                if (BindingTokens[i] == "*")
                {
                    if (MethodTokens[j] == "?")
                        return false; // cannot match /aaa/* to /aaa/?, because /aaa/? is wider.
                    else
                        continue; // it is a local wild card - anything goes
                }
                if (BindingTokens[i].StartsWith("{"))
                    continue; // this is a url parameter, which is for the purpose of matching as good as a local wild card
                // note, that it is sufficient to check just the first character. The presence of the last one
                // is ensured by the regular expression in binding parser  
                if (BindingTokens[i] == "?")
                {
                    // it is a global wild card  - we need to find a matching tail
                    if (++i >= BindingTokens.Length)
                        return true;  // there is no tail to match - we can report a match
                    bool found = false;
                    for (; j < MethodTokens.Length; j++)
                        if (BindingTokens[i] == MethodTokens[j])
                        {
                            found = true;
                            break;
                        }
                    if (found)
                        continue;
                }
                return false; // no match
            }

            if (i < BindingTokens.Length) // items left in the binding
                for (; i < BindingTokens.Length; i++) // if all of them are wild cards this is a match
                    if (BindingTokens[i] != "*" && BindingTokens[i] != "?" && !BindingTokens[i].StartsWith("{"))
                        return false;

            // There is an implicit /? in the end of every binding
            //if (j < MethodTokens.Length) // items left in the method url - no match
            //    return false;

            return true;
        }

        

        private Resource lookupResource(string resourceName)
        {
            Resource result;
            if (!resources.TryGetValue(resourceName, out result))
            {
                result = new Resource(engine, this, resourceName);
                resources.Add(resourceName, result);
            }
            return result;
        }

        //internal void Register(Controller controller)
        //{
        //    controllers.Add(controller);
        //    controller.Type.Register(controller);
        //    foreach (string resourceName in controller.Type.Provides)
        //        lookupResource(resourceName).AddProvider(controller.Type);
        //    foreach (string resourceName in controller.Type.Requires)
        //        lookupResource(resourceName).AddRequiredBy(controller.Type);
        //    foreach (string resourceName in controller.Type.DependsOn)
        //        lookupResource(resourceName).AddDependents(controller.Type);
        //}

        internal void Register(Binding binding,ControllerType ctrType, string methodUrl)
        {
            foreach (Controller ctr in controllers)
            {
                if (ctr.Type == ctrType)
                    return;
            }
            Controller controller = new Controller(binding, ctrType, methodUrl);
            controllers.Add(controller);
            ctrType.Register(controller);
            foreach (string resourceName in controller.Type.Provides)
                lookupResource(resourceName).AddProvider(controller.Type);
            foreach (string resourceName in controller.Type.Requires)
                lookupResource(resourceName).AddRequiredBy(controller.Type);
            foreach (string resourceName in controller.Type.DependsOn)
                lookupResource(resourceName).AddDependents(controller.Type);
        }


        internal void Unregister(Controller controller)
        {
            foreach (Resource resource in new List<Resource>(resources.Values))
                if (resource.IsEmpty)
                    resources.Remove(resource.Name);
            controllers.Remove(controller);
            checkAndRemove();            
        }

        private void checkAndRemove()
        {
            if (this == engine.Root)
                // this is the root node
                return;

            if (controllers.Count > 0)
                // we still have controllers left - it is still a viable method
                return;

            if (bindings.Count > 1)
            {
                // there are 2 or more bindings hanging off this binding  
                return;
            }

            if (bindings.Count == 1)
            // this node is redundant - it has the only child and no controllers 
            // so this is not a method let us recombine the binding
            {
                Binding child = bindings[0];
                parent.Adopt(child, bindingUrl + child.bindingUrl);
                parent.RegisterBindings(child, this);
                // there is no need for parent check and remove call here
                // the parent will always be viable at this point. Later
                // if the recombined child itself will be removed, this will remove
                // the parent as well (if the child will be the last one at this point)
                Dispose();
            }
            else
            {
                Dispose();
                parent.checkAndRemove();
            }
        }

        private void Dispose()
        {
            parent.bindings.Remove(this);
            parent.RegisterBindings(null, this);
        }

        internal void Validate()
        {
            foreach (Binding binding in bindings)
                binding.Validate();
            foreach (Resource resource in resources.Values)
                resource.Validate();

            DependencyGraph graph = new DependencyGraph(controllers);
            foreach (Resource resource in resources.Values)
                foreach (ControllerType providingController in resource.Providers)
                {
                    foreach (ControllerType consumer in resource.Dependents)
                        graph.AddEdge(consumer, providingController);
                    foreach (ControllerType consumer in resource.RequiredBy)
                        graph.AddEdge(consumer, providingController);
                }
            if (!graph.TopologicalSort()) 
                engine.RaiseResourceLoop(verb + ' ' + fullBindingUrl, controllers);
        }
    }
}
