using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using FSharp.ProjectExtender;
using System.Threading;

namespace IntegrationTests
{
    

    [TestClass]
    public class ExtenderTests
    {
        #region fields
        private delegate void ThreadInvoker();
        /// <summary>
        /// TextContext Instances: 
        /// TestContexts are different for each test method, with no sharing between test methods

        /// </summary>
        private static TestContext testContext;
        #endregion

        #region properties
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        /*public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }*/
        #endregion

        #region ctors
        public ExtenderTests()
        {
        }
        #endregion

        #region Additional test attributes
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void ExtenderInitialize(TestContext ctx) 
        {
            testContext = ctx;
            string path = ctx.TestDir.Substring(0, ctx.TestDir.IndexOf("TestResults"));
            testContext.Properties.Add("slnfile",path + "ConsoleApplication3\\ConsoleApplication3.sln");
            testContext.Properties.Add("projfile", path + "ConsoleApplication3\\ConsoleApplication3\\ConsoleApplication3.fsproj");
            testContext.Properties.Add("testfile", path + "ConsoleApplication3\\ConsoleApplication3\\ConsoleApplication3_test.fsproj");
            testContext.Properties.Add("solution", null);
            testContext.Properties.Add("hierarchy", null);
            testContext.Properties.Add("viewer", null);
            
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void ExtenderCleanup() 
        {
            testContext.Properties.Clear();
        }

        // Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        /*public void ControlInitialize() 
        {
              File.Copy(testContext.Properties["projfile"].ToString(), testContext.Properties["testfile"].ToString(), true);
              IVsHierarchy hier;
              IVsSolution sln = VsIdeTestHostContext.ServiceProvider.GetService(typeof(IVsSolution)) as IVsSolution;
              sln.OpenSolutionFile((uint)__VSSLNOPENOPTIONS.SLNOPENOPT_Silent, testContext.Properties["slnfile"].ToString());
              sln.GetProjectOfUniqueName(testContext.Properties["testfile"].ToString(), out hier);
              Assert.IsNotNull(hier,"Project is not IProjectManager");
              CompileOrderViewer viewer = new CompileOrderViewer((IProjectManager)hier);
              Assert.IsNotNull(viewer, "Fail to create Viewer");
              testContext.Properties["viewer"] = viewer;
              testContext.Properties["solution"] = sln;
              testContext.Properties["hierarchy"] = hier;
        }*/

        // Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        /*public void ControlCleanup() 
        {
              ((IProjectManager)testContext.Properties["hierarchy"]).BuildManager.FixupProject();
              ((CompileOrderViewer)testContext.Properties["viewer"]).Dispose();
              ((IVsSolution)testContext.Properties["solution"]).CloseSolutionElement
                  ((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave, null, 0);


        }*/
        
        #endregion


        [TestMethod]
        [HostType("VS IDE")]
        public void InsideSameFolder()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                new SwapConfig("InsideSameFolder", ref testContext)
                                    .Move(
                                         new MoveOp {Index = 0, Dir = CompileOrderViewer.Direction.Down},
                                         new MoveOp {Index = 2, Dir = CompileOrderViewer.Direction.Down})
                                    .ExpectedOrder(
                                        "Folder1\\File4.fs",
                                        "Folder1\\File3.fs",
                                        "Folder1\\Sub1\\SubSub1\\File1.fs",
                                        "Folder1\\Sub1\\File2.fs",
                                        "Folder1\\File1.fs",
                                        "Program.fs",
                                        "Program2.fs",
                                        "Program3.fs")
                                    .Run();
            });
        }
        [TestMethod]
        [HostType("VS IDE")]
        public void BetweenDiffFolders()
        {
            ///"Folder1\\File3.fs",
            ///"Folder1\\File4.fs",
            ///"Folder1\\Sub1\\File2.fs",
            ///"Folder1\\Sub1\\SubSub1\\File1.fs",
            ///"Folder1\\File1.fs",
            ///"Program.fs"

            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                new SwapConfig("BetweenDiffFolders", ref testContext)
                                        .Move(
                                              new[]
                                           {
                                               new MoveOp {Index = 2, Dir = CompileOrderViewer.Direction.Up},
                                               new MoveOp {Index = 1, Dir = CompileOrderViewer.Direction.Up},
                                           })
                                        .ExpectedOrder(
                                            "Folder1\\Sub1\\File2.fs",
                                            "Folder1\\File3.fs",
                                            "Folder1\\File4.fs",
                                            "Folder1\\Sub1\\SubSub1\\File1.fs",
                                            "Folder1\\File1.fs",
                                            "Program.fs",
                                            "Program2.fs",
                                            "Program3.fs")
                                        .Run();
            });

           
        }
        [TestMethod]
        [HostType("VS IDE")]
        public void PickToUp()
        {
            ///"Folder1\\File3.fs",
            ///"Folder1\\File4.fs",
            ///"Folder1\\Sub1\\File2.fs",
            ///"Folder1\\Sub1\\SubSub1\\File1.fs",
            ///"Folder1\\File1.fs",
            ///"Program.fs"
            ///"Program2.fs"
            ///"Program3.fs"
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                new SwapConfig("PickToUp", ref testContext)
                                        .Move(
                                              new[]
                                           {
                                               new MoveOp {Index = 5, Dir = CompileOrderViewer.Direction.Up},
                                               new MoveOp {Index = 4, Dir = CompileOrderViewer.Direction.Up},
                                               new MoveOp {Index = 3, Dir = CompileOrderViewer.Direction.Up},
                                               new MoveOp {Index = 2, Dir = CompileOrderViewer.Direction.Up},
                                               new MoveOp {Index = 1, Dir = CompileOrderViewer.Direction.Up},
                                           })
                                        .ExpectedOrder(
                                            "Program.fs",
                                            "Folder1\\File3.fs",
                                            "Folder1\\File4.fs",
                                            "Folder1\\Sub1\\File2.fs",
                                            "Folder1\\Sub1\\SubSub1\\File1.fs",
                                            "Folder1\\File1.fs",
                                            "Program2.fs",
                                            "Program3.fs"
                                            )
                                        .Run();
            });


        }

        /*[TestMethod]
        [Ignore]
        [HostType("VS IDE")]
        public void CreateProjectStructure()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                TestUtils testUtils = new TestUtils();
                testUtils.CreateEmptySolution(testContext.TestDir, "Test1");
                string projName = "FSharpExApp1";
                string projfullName = testUtils.CreateProjectFromTemplate(projName, "F# Application", "FSharp", false);
                projfullName += "\\" + projName + ".fsproj";
                Assert.AreEqual<int>(1, testUtils.ProjectCount());
            });
        }*/
        /*[TestMethod]
        [Ignore]
        [HostType("VS IDE")]
        public void ShowPageTest()
        {
            IVsWindowFrame frame;
            IVsUIShell shell = VsIdeTestHostContext.ServiceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
            Guid guidPropertyBrowser = new Guid(ToolWindowGuids.PropertyBrowser);
            shell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref guidPropertyBrowser, out frame);
            frame.Show();
            IVsRunningDocumentTable rdt = (IVsRunningDocumentTable)VsIdeTestHostContext.ServiceProvider.GetService(typeof(SVsRunningDocumentTable));
            IVsHierarchy hier;
            uint itemId;
            IntPtr docData;
            //rdt.FindAndLockDocument(_VSRDTFLAGS.RDT_NoLock, projfile, out hier, out itemId, out docData); 
            //IPropertyPage page = (IPropertyPage)Marshal.GetObjectForIUnknown(docData);
            //Assert.IsNotNull(page, "Property page is null");

        }*/
    }
}
