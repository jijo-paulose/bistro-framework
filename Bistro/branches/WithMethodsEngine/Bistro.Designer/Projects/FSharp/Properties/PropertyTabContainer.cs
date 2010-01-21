using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Bistro.Designer.ProjectBase;
using Microsoft.VisualStudio.OLE.Interop;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Microsoft.VisualStudio;
using System.Collections;
using Microsoft.VisualStudio.Shell.Interop;

namespace Bistro.Designer.Projects.FSharp
{
	[CLSCompliant(false), ComVisible(true)]
    public abstract class PropertyTabContainer<T>:  //SettingsPage,
        LocalizableProperties,
        IPropertyPage
        where T : Control
    {
        private T control;
        private bool dirty;
        private bool active;
        private IPropertyPageSite site;
        private ProjectManager project;
        private ProjectConfig[] projectConfigs;

        protected abstract void BindProperties();
        protected abstract int ApplyChanges();
        protected abstract string Name { get; }
        protected abstract T CreateControl();

        protected bool IsDirty
        {
            get
            {
                return this.dirty;
            }
            set
            {
                if (this.dirty != value)
                {
                    this.dirty = value;
                    if (this.site != null)
                        site.OnStatusChange((uint)(this.dirty ? PropPageStatus.Dirty : PropPageStatus.Clean));
                }
            }
        }

        protected T Control { get { return control; } }

        protected ProjectManager Project { get { return project; } } 

        #region public methods
        public object GetTypedConfigProperty(string name, Type type)
        {
            string value = GetConfigProperty(name);
            if (string.IsNullOrEmpty(value)) return null;

            TypeConverter tc = TypeDescriptor.GetConverter(type);
            return tc.ConvertFromInvariantString(value);
        }

        public object GetTypedProperty(string name, Type type)
        {
            string value = GetProperty(name);
            if (string.IsNullOrEmpty(value)) return null;

            TypeConverter tc = TypeDescriptor.GetConverter(type);
            return tc.ConvertFromInvariantString(value);
        }

        public string GetProperty(string propertyName)
        {
            //if (this.project != null)
            //{
            //    string property = this.project.BuildProject.GlobalProperties[propertyName].Value;

            //    if (property != null)
            //    {
            //        return property;
            //    }
            //}
            return String.Empty;
        }

        // relative to active configuration.
        public string GetConfigProperty(string propertyName)
        {
            if (this.project != null)
            {
                string unifiedResult = null;
                bool cacheNeedReset = true;

                for (int i = 0; i < this.projectConfigs.Length; i++)
                {
                    ProjectConfig config = projectConfigs[i];
                    string property = config.GetConfigurationProperty(propertyName, cacheNeedReset);
                    cacheNeedReset = false;

                    if (property != null)
                    {
                        string text = property.Trim();

                        if (i == 0)
                            unifiedResult = text;
                        else if (unifiedResult != text)
                            return ""; // tristate value is blank then
                    }
                }

                return unifiedResult;
            }

            return String.Empty;
        }

        /// <summary>
        /// Sets the value of a configuration dependent property.
        /// If the attribute does not exist it is created.  
        /// If value is null it will be set to an empty string.
        /// </summary>
        /// <param name="name">property name.</param>
        /// <param name="value">value of property</param>
        public void SetConfigProperty(string name, string value)
        {
            CCITracing.TraceCall();
            if (value == null)
            {
                value = String.Empty;
            }

            if (this.project != null)
            {
                for (int i = 0, n = this.projectConfigs.Length; i < n; i++)
                {
                    ProjectConfig config = projectConfigs[i];

                    config.SetConfigurationProperty(name, value);
                }

                //this.project.SetProjectFileDirty(true);
            }
        }
        #endregion

        #region IPropertyPage methods.
        public virtual void Activate(IntPtr parent, RECT[] pRect, int bModal)
        {
            if (this.control == null)
            {
                this.control = CreateControl();
                this.control.Size = new Size(pRect[0].right - pRect[0].left, pRect[0].bottom - pRect[0].top);
                this.control.Visible = false;
                this.control.Size = new Size(550, 300);
                this.control.CreateControl();
                NativeMethods.SetParent(this.control.Handle, parent);
                BindProperties();
                this.active = true;
            }
        }

        public virtual int Apply()
        {
            if (IsDirty)
            {
                if (project == null)
                    return VSConstants.E_INVALIDARG;
                var result = this.ApplyChanges();
                if (result == VSConstants.S_OK)
                    IsDirty = false;
                return result;
            }
            return VSConstants.S_OK;
        }

        public virtual void Deactivate()
        {
            if (null != this.control)
            {
                this.control.Dispose();
                this.control = null;
            }
            this.active = false;
        }

        public virtual void GetPageInfo(PROPPAGEINFO[] arrInfo)
        {
            PROPPAGEINFO info = new PROPPAGEINFO();

            info.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
            info.dwHelpContext = 0;
            info.pszDocString = null;
            info.pszHelpFile = null;
            info.pszTitle = this.Name;
            info.SIZE.cx = 550;
            info.SIZE.cy = 300;
            arrInfo[0] = info;
        }

        public virtual void Help(string helpDir)
        {
        }

        public virtual int IsPageDirty()
        {
            // Note this returns an HRESULT not a Bool.
            return (IsDirty ? (int)VSConstants.S_OK : (int)VSConstants.S_FALSE);
        }

        public virtual void Move(RECT[] arrRect)
        {
            RECT r = arrRect[0];

            this.control.Location = new Point(r.left, r.top);
            this.control.Size = new Size(r.right - r.left, r.bottom - r.top);
        }

        public virtual void SetObjects(uint count, object[] punk)
        {
            //if (count > 0)
            //{
            //    if (punk[0] is ProjectConfig)
            //    {
            //        ArrayList configs = new ArrayList();
            //        for (int i = 0; i < count; i++)
            //        {
            //            ProjectConfig config = (ProjectConfig)punk[i];
            //            if (this.project == null)
            //            {
            //                this.project = config.ProjectMgr as ProjectManager;
            //            }
            //            configs.Add(config);
            //        }
            //        this.projectConfigs = (ProjectConfig[])configs.ToArray(typeof(ProjectConfig));
            //    }
            //    else if (punk[0] is NodeProperties)
            //    {
            //        if (this.project == null)
            //        {
            //            this.project = (punk[0] as NodeProperties).Node.ProjectMgr as ProjectManager;
            //        }

            //        var configsMap = new System.Collections.Generic.Dictionary<string, ProjectConfig>();
            //        for (int i = 0; i < count; i++)
            //        {
            //            NodeProperties property = (NodeProperties)punk[i];
            //            IVsCfgProvider provider;
            //            ErrorHandler.ThrowOnFailure(property.Node.ProjectMgr.GetCfgProvider(out provider));
            //            uint[] expected = new uint[1];
            //            ErrorHandler.ThrowOnFailure(provider.GetCfgs(0, null, expected, null));
            //            if (expected[0] > 0)
            //            {
            //                ProjectConfig[] configs = new ProjectConfig[expected[0]];
            //                uint[] actual = new uint[1];
            //                ErrorHandler.ThrowOnFailure(provider.GetCfgs(expected[0], configs, actual, null));

            //                foreach (ProjectConfig config in configs)
            //                {
            //                    if (!configsMap.ContainsKey(config.ConfigName))
            //                    {
            //                        configsMap.Add(config.ConfigName, config);
            //                    }
            //                }
            //            }
            //        }

            //        if (configsMap.Count > 0)
            //        {
            //            if (this.projectConfigs == null)
            //            {
            //                this.projectConfigs = new ProjectConfig[configsMap.Keys.Count];
            //            }
            //            configsMap.Values.CopyTo(this.projectConfigs, 0);
            //        }
            //    }
            //}
            //else
            {
                this.project = null;
            }

            if (this.active && this.project != null)
            {
                BindProperties();
            }
        }

        public virtual void SetPageSite(IPropertyPageSite theSite)
        {
            this.site = theSite;
        }

        public virtual void Show(uint cmd)
        {
            this.control.Visible = true; // TODO: pass SW_SHOW* flags through      
            this.control.Show();
        }

        public virtual int TranslateAccelerator(MSG[] arrMsg)
        {
            MSG msg = arrMsg[0];

            if ((msg.message < NativeMethods.WM_KEYFIRST || msg.message > NativeMethods.WM_KEYLAST) && (msg.message < NativeMethods.WM_MOUSEFIRST || msg.message > NativeMethods.WM_MOUSELAST))
                return 1;

            return (NativeMethods.IsDialogMessageA(this.control.Handle, ref msg)) ? 0 : 1;
        }

        #endregion

    }
}
