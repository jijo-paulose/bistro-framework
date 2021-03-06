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
            testContext.Properties.Add("slnfile", path + "ConsoleApplication3\\ConsoleApplication3.sln");
            testContext.Properties.Add("suo", path + "ConsoleApplication3\\ConsoleApplication3.suo");
            testContext.Properties.Add("projfile", path + "ConsoleApplication3\\ConsoleApplication3\\ConsoleApplication3.fsproj");
            testContext.Properties.Add("testfile", path + "ConsoleApplication3\\ConsoleApplication3\\ConsoleApplication3_test.fsproj");

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

        #region Just check compile items order after random swap
        [TestMethod]
        [HostType("VS IDE")]
        public void InsideSameFolder()
        {
            //"Program3.fs"
            //"Folder1\\File3.fs"
            //"Folder1\\File4.fs"
            //"Folder1\\Sub1\File2.fs"
            //"Folder1\\Sub1\\SubSub1\\File1.fs"
            //"Folder1\\Sub2\\File1.fs"
            //"Folder1\\File1.fs"
            //"Folder2\\File1.fs"
            //"Program.fs"
            //"Program2.fs"
            //"Folder3\\CompileFile1.fs",
            //"Folder3\\ContentFile1.fs",
            //"Folder3\\NoneFile1.fs"

            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                //"It'd be more optimal if root level files won't be swapped in this tests.
                //But they are swapped (manual check,not yet automized)
                new SwapConfig("InsideSameFolder", ref testContext)
                                    .Move(
                                         new MoveOp { Index = 1, Dir = CompileOrderViewer.Direction.Down },
                                         new MoveOp { Index = 3, Dir = CompileOrderViewer.Direction.Down })
                                    .ExpectedOrder(
                                        "Program3.fs",
                                        "Folder1\\File4.fs",
                                        "Folder1\\File3.fs",
                                        "Folder1\\Sub1\\SubSub1\\File1.fs",
                                        "Folder1\\Sub1\\File2.fs",
                                        "Folder1\\Sub2\\File1.fs",
                                        "Folder1\\File1.fs",
                                        "Folder2\\File1.fs",
                                        "Program.fs",
                                        "Program2.fs",
                                        "Folder3\\CompileFile1.fs"//,
                                        //"Folder3\\ContentFile1.fs",
                                        //"Folder3\\NoneFile1.fs"
                                        )
                                    .Run();
            });
        }
        [TestMethod]
        [HostType("VS IDE")]
        [Description("This test will fail if string path = '\\' + Path.GetDirectoryName(item.Path) in FixUp")]
        public void BetweenDiffFolders1()
        {
            //"Program3.fs"
            //"Folder1\\File3.fs"
            //"Folder1\\File4.fs"
            //"Folder1\\Sub1\File2.fs"
            //"Folder1\\Sub1\\SubSub1\\File1.fs"
            //"Folder1\\Sub2\\File1.fs"
            //"Folder1\\File1.fs"
            //"Folder2\\File1.fs"
            //"Program.fs"
            //"Program2.fs"
            //"Folder3\\CompileFile1.fs",
            //"Folder3\\ContentFile1.fs",
            //"Folder3\\NoneFile1.fs"


            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                new SwapConfig("BetweenDiffFolders", ref testContext)
                                        .Move(
                                               new MoveOp { Index = 3, Dir = CompileOrderViewer.Direction.Up },
                                               new MoveOp { Index = 2, Dir = CompileOrderViewer.Direction.Up },
                                               new MoveOp { Index = 1, Dir = CompileOrderViewer.Direction.Up }
                                         )
                                        .ExpectedOrder(
                                            "Folder1\\Sub1\\File2.fs",
                                            "Program3.fs",
                                            "Folder1\\File3.fs",
                                            "Folder1\\File4.fs",
                                            "Folder1\\Sub1\\SubSub1\\File1.fs",
                                            "Folder1\\Sub2\\File1.fs",
                                            "Folder1\\File1.fs",
                                            "Folder2\\File1.fs",
                                            "Program.fs",
                                            "Program2.fs",
                                            "Folder3\\CompileFile1.fs"//,
                                            //"Folder3\\ContentFile1.fs",
                                            //"Folder3\\NoneFile1.fs"
                                        )
                                        .Run();
            });


        }
        [TestMethod]
        [HostType("VS IDE")]
        public void BetweenDiffFolders2()
        {
            //"Program3.fs"
            //"Folder1\\File3.fs"
            //"Folder1\\File4.fs"
            //"Folder1\\Sub1\File2.fs"
            //"Folder1\\Sub1\\SubSub1\\File1.fs"
            //"Folder1\\Sub2\\File1.fs"
            //"Folder1\\File1.fs"
            //"Folder2\\File1.fs"
            //"Program.fs"
            //"Program2.fs"
            //"Folder3\\CompileFile1.fs",
            //"Folder3\\ContentFile1.fs",
            //"Folder3\\NoneFile1.fs"
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                new SwapConfig("BetweenDiffFolders2", ref testContext)
                                        .Move(
                                               new MoveOp { Index = 2, Dir = CompileOrderViewer.Direction.Down },
                                               new MoveOp { Index = 7, Dir = CompileOrderViewer.Direction.Up },
                                               new MoveOp { Index = 6, Dir = CompileOrderViewer.Direction.Up },
                                               new MoveOp { Index = 5, Dir = CompileOrderViewer.Direction.Up }
                                         )
                                        .ExpectedOrder(
                                               "Program3.fs",
                                               "Folder1\\File3.fs",
                                               "Folder1\\Sub1\\File2.fs",
                                               "Folder1\\File4.fs",
                                               "Folder2\\File1.fs",
                                               "Folder1\\Sub1\\SubSub1\\File1.fs",
                                               "Folder1\\Sub2\\File1.fs",
                                               "Folder1\\File1.fs",
                                               "Program.fs",
                                               "Program2.fs",
                                               "Folder3\\CompileFile1.fs"//,
                                               //"Folder3\\ContentFile1.fs",
                                               //"Folder3\\NoneFile1.fs"
                                        )
                                        .Run();
            });


        }


        [TestMethod]
        [HostType("VS IDE")]
        public void PullDown()
        {
  
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                new SwapConfig("PullDown", ref testContext)
                                        .Move(
                                                new MoveOp { Index = 4, Dir = CompileOrderViewer.Direction.Down },
                                                new MoveOp { Index = 5, Dir = CompileOrderViewer.Direction.Down },
                                                new MoveOp { Index = 6, Dir = CompileOrderViewer.Direction.Down },
                                                new MoveOp { Index = 7, Dir = CompileOrderViewer.Direction.Down },
                                                new MoveOp { Index = 9, Dir = CompileOrderViewer.Direction.Down },
                                                new MoveOp { Index = 7, Dir = CompileOrderViewer.Direction.Up }

                                           )
                                        .ExpectedOrder(
                                                "Program3.fs",
                                                "Folder1\\File3.fs",
                                                "Folder1\\File4.fs",
                                                "Folder1\\Sub1\\File2.fs",
                                                "Folder1\\Sub2\\File1.fs",
                                                "Folder1\\File1.fs",
                                                "Program.fs",
                                                "Folder2\\File1.fs",
                                                "Folder1\\Sub1\\SubSub1\\File1.fs",
                                                "Folder3\\CompileFile1.fs",
                                                //"Folder3\\ContentFile1.fs",
                                                //"Folder3\\NoneFile1.fs",
                                                "Program2.fs"

                                            )
                                        .Run();
            });


        }
        #endregion
        #region Compile,None,Content order
        [TestMethod]
        [Description("The test swaps file stored in the folder where items with 'Content','None' build action exist")]
        [HostType("VS IDE")]
        public void NoneFilesOrder()
        {
            //[0]"Program3.fs",
            //[1]"Folder1\\File3.fs",
            //[2]"Folder1\\File4.fs",
            //[3]"Folder1\\Sub1\File2.fs",
            //[4]"Folder1\\Sub1\\SubSub1\\File1.fs",
            //[5]"Folder1\\Sub2\\File1.fs",
            //[6]"Folder1\\File1.fs",
            //[7]"Folder2\\File1.fs",
            //[8]"Program.fs",
            //[9]"Program2.fs",
            //[10]"Folder3\\CompileFile1.fs",
            //[11]"Folder3\\ContentFile1.fs",
            //[12]"Folder3\\NoneFile1.fs"
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                new SwapConfig("NoneFilesOrder", ref testContext)
                                        .Move(
                                            new MoveOp { Index = 10, Dir = CompileOrderViewer.Direction.Up },
                                            new MoveOp { Index = 9, Dir = CompileOrderViewer.Direction.Up },
                                            new MoveOp { Index = 8, Dir = CompileOrderViewer.Direction.Up },
                                            new MoveOp { Index = 7, Dir = CompileOrderViewer.Direction.Up },
                                            new MoveOp { Index = 6, Dir = CompileOrderViewer.Direction.Up }
                                        )
                                        .ExpectedOrder(
                                            "Program3.fs",
                                            "Folder1\\File3.fs",
                                            "Folder1\\File4.fs",
                                            "Folder1\\Sub1\\File2.fs",
                                            "Folder1\\Sub1\\SubSub1\\File1.fs",
                                            "Folder3\\CompileFile1.fs",
                                            "Folder1\\Sub2\\File1.fs",
                                            //"Folder3\\ContentFile1.fs",
                                            //"Folder3\\NoneFile1.fs"
                                            "Folder1\\File1.fs",
                                            "Folder2\\File1.fs",
                                            "Program.fs",
                                            "Program2.fs"//,
                                        )
                                        .Run();
            });
        }

        #endregion

        [TestMethod]
        [HostType("VS IDE")]
        //[Ignore]
        /*[Description
         ("No changes made.Project file should not be changed after FixUp.If this test fails,the algorithm realization is non-optimal"
         )]*/
        [Description("Now it fails because items of the root level 'are always joined'" )]
        public void NoChangesMade()
        {
            //[0]"Program3.fs",
            //[1]"Folder1\\File3.fs",
            //[2]"Folder1\\File4.fs",
            //[3]"Folder1\\Sub1\File2.fs",
            //[4]"Folder1\\Sub1\\SubSub1\\File1.fs",
            //[5]"Folder1\\Sub2\\File1.fs",
            //[6]"Folder1\\File1.fs",
            //[7]"Folder2\\File1.fs",
            //[8]"Program.fs",
            //[9]"Program2.fs",
            //[10]"Folder3\\CompileFile1.fs",
            //[11]"Folder3\\ContentFile1.fs",
            //[12]"Folder3\\NoneFile1.fs"
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                SwapConfig swap = new SwapConfig("NoneFilesOrder", ref testContext);
                swap.ExpectedOrder(
                         "Program3.fs",
                         "Folder1\\File3.fs",
                         "Folder1\\File4.fs",
                         "Folder1\\Sub1\\File2.fs",
                         "Folder1\\Sub1\\SubSub1\\File1.fs",
                         "Folder1\\Sub2\\File1.fs",
                         "Folder1\\File1.fs",
                         "Folder2\\File1.fs",
                         "Program.fs",
                         "Program2.fs",
                         "Folder3\\CompileFile1.fs"//,
                         //"Folder3\\ContentFile1.fs",
                         //"Folder3\\NoneFile1.fs"
                         );
                swap.NoneChanged();


            });
        }

        [TestMethod]
        [HostType("VS IDE")]
        [Description("Validating reordering of files in the root directory")]
        public void FilesInRoot()
        {
            //[0]"Program3.fs",
            //[1]"Folder1\\File3.fs",
            //[2]"Folder1\\File4.fs",
            //[3]"Folder1\\Sub1\File2.fs",
            //[4]"Folder1\\Sub1\\SubSub1\\File1.fs",
            //[5]"Folder1\\Sub2\\File1.fs",
            //[6]"Folder1\\File1.fs",
            //[7]"Folder2\\File1.fs",
            //[8]"Program.fs",
            //[9]"Program2.fs",
            //[10]"Folder3\\CompileFile1.fs",
            //[11]"Folder3\\ContentFile1.fs",
            //[12]"Folder3\\NoneFile1.fs"
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                SwapConfig swap = new SwapConfig("FilesInRoot", ref testContext);
                swap.Move(new MoveOp{ Index=0, Dir = CompileOrderViewer.Direction.Down })
                    .ExpectedOrder(
                         "Folder1\\File3.fs",
                         "Program3.fs",
                         "Folder1\\File4.fs",
                         "Folder1\\Sub1\\File2.fs",
                         "Folder1\\Sub1\\SubSub1\\File1.fs",
                         "Folder1\\Sub2\\File1.fs",
                         "Folder1\\File1.fs",
                         "Folder2\\File1.fs",
                         "Program.fs",
                         "Program2.fs",
                         "Folder3\\CompileFile1.fs"//,
                    //"Folder3\\ContentFile1.fs",
                    //"Folder3\\NoneFile1.fs"
                         );
                swap.Run();


            });
        }
    }
}
