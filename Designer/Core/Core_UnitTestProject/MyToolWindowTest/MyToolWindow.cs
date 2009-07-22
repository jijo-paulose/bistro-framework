/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections;
using System.Text;
using System.Reflection;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bistro.Designer.Explorer;

namespace UnitTestProject.MyToolWindowTest
{
    /// <summary>
    ///This is a test class for MyToolWindowTest and is intended
    ///to contain all MyToolWindowTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ToolWindowTest
    {

        /// <summary>
        ///MyToolWindow Constructor test
        ///</summary>
        [TestMethod()]
        public void MyToolWindowConstructorTest()
        {
            ToolWindow target = new ToolWindow();
            Assert.IsNotNull(target, "Failed to create an instance of MyToolWindow");

            FieldInfo field = target.GetType().GetField("control", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(field.GetValue(target), "Explorer.Control object was not instantiated");
        }

        public void WindowPropertyTest()
        {
            ToolWindow target = new ToolWindow();
            Assert.IsNotNull(target.Window, "Window property was null");
        }

    }
}
