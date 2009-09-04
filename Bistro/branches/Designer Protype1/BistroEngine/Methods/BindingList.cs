using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Methods
{
    internal class BindingList 
    {
        SortedDictionary<string, Binding> dictionary = new SortedDictionary<string, Binding>();

        private string buildKey(Binding binding) {return binding.Verb + ';' + binding.BindingUrl;}

        public IEnumerable<Binding> Values { get { return dictionary.Values; } }

        public int Count { get { return dictionary.Count; } }

        public void Add(Binding binding) { dictionary.Add(buildKey(binding), binding); }

        public void Remove(Binding binding) { dictionary.Remove(buildKey(binding)); }
    }
}
