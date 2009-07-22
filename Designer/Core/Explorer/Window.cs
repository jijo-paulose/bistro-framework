using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;
using Bistro.Designer.Core;

namespace Bistro.Designer.Explorer
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsWindowPane interface.
    /// </summary>
    [Guid("3f3c1dea-5502-43c5-9ccd-e2e642aae265")]
    public class ToolWindow : ToolWindowPane
    {
        // This is the user control hosted by the tool window; it is exposed to the base class 
        // using the Window property. Note that, even if this class implements IDispose, we are
        // not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
        // the object returned by the Window property.
        private Control control;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public ToolWindow() :
            base(null)
        {
            // Set the window title reading it from the resources.
            this.Caption = Bistro.Designer.Core.Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;


            control = new Control();
        }

        /// <summary>
        /// This property returns the handle to the user control that should
        /// be hosted in the Tool Window.
        /// </summary>
        override public IWin32Window Window
        {
            get
            {
                return (IWin32Window)control;
            }
        }

        /// <summary>
        /// Initializes Bistro Explorer service  
        /// </summary>
        /// <param name="package"></param>
        public static void Intialize(CorePackage package)
        {
            OleMenuCommandService commandService = ((IServiceProvider)package).GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            // Create the command for the show explorer command
            commandService.AddCommand(
                new MenuCommand(
                    (object sender, EventArgs e) =>
                    {
                        // Get the instance number 0 of this tool window. This window is single instance so this instance
                        // is actually the only one.
                        // The last flag is set to true so that if the tool window does not exists it will be created.
                        ToolWindowPane window = package.FindToolWindow(typeof(ToolWindow), 0, true);
                        if ((null == window) || (null == window.Frame))
                        {
                            throw new NotSupportedException(Resources.CanNotCreateWindow);
                        }
                        IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
                        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
                    },
                    new CommandID(Guids.guidCoreCmdSet, (int)PkgCmdIDList.cmdidShowExplorer)
                    )
                );
        }

    }
}
