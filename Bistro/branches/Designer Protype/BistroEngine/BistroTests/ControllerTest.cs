using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bistro.Methods;

namespace Bistro.Tests
{
    internal class ControllerTest
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
            Assert.AreEqual(type, controller.Type.Name, "Method " + url + ": Controller Type mismatch");
//            Assert.AreEqual(seq, controller.SeqNumber, "Method " + url + " controller " + type + ": Controller sequence mismatch");
        }
    }
}
