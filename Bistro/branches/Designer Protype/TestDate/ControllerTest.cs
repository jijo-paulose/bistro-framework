using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Methods;

namespace Bistro.Tests
{
    public class ControllerTest
    {
        public ControllerTest(string type, int seq)
        {
            this.type = type;
            this.seq = seq;
        }
        string type;
        int seq;

        public void Validate(string url, Controller controller)
        {
        }
    }
}
