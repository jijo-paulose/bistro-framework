using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using WorkflowServer.Bistro.Designer.Explorer;

namespace Bistro.Methods
{
    public class Controller
    {
        public Controller(Binding binding, Controller source)
        {
            type = source.type;
            this.binding = binding;
        }

        public Controller(Binding binding, ControllerType type, string methodUrl)
        {
            this.type = type;
            this.binding = binding;
        }

        ControllerType type;
        Binding binding;

        public ControllerType Type { get { return type; } }

        int seqNumber;
        public int SeqNumber { get { return seqNumber; } set { seqNumber = value; } }

        internal void Dispose()
        {
           binding.Unregister(this);
        }
    }
}
