using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Bistro.Designer.ProjectBase;
using Microsoft.VisualStudio;
using System.Windows.Forms;

namespace Bistro.Designer.Projects.FSharp.Properties
{
    [CLSCompliant(false), ComVisible(true), Guid("E04AA22F-EC71-4a64-A7B5-8537F85A341E")]
    public class ApplicationPage : PropertyTabContainer
    {
        string rootNamespace;

        #region properties
        [SRCategoryAttribute(FSharpPropertiesConstants.Application)]
        [LocDisplayName(ProjectFileConstants.RootNamespace)]
        [SRDescriptionAttribute(FSharpPropertiesConstants.RootNamespaceDescription)]
        public string RootNamespace
        {
            get { return this.rootNamespace; }
            set { this.rootNamespace = value; this.IsDirty = true; }
        }
        #endregion

        public ApplicationPage()
        {
            Name = SR.GetString(FSharpPropertiesConstants.Application);
        }


        ApplicationViewer control;
        protected override Control CreateControl()
        {
            control = new ApplicationViewer(this);
            return control;
        }

        protected override void BindProperties()
		{
            rootNamespace = GetConfigProperty(ProjectFileConstants.RootNamespace);
            control.BindProperties();
		}

		protected override int ApplyChanges()
		{
			if(ProjectMgr == null)
				return VSConstants.E_INVALIDARG;

			SetConfigProperty(ProjectFileConstants.RootNamespace, rootNamespace);
			IsDirty = false;
			return VSConstants.S_OK;
		}
    }
}
