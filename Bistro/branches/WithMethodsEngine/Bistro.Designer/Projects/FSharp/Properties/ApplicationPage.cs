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
    public class ApplicationPage : PropertyTabContainer<ApplicationViewer>
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

        protected override string Name
        {
            get { return FSharpPropertiesConstants.Application; }
        }

        protected override ApplicationViewer CreateControl()
        {
            return new ApplicationViewer(this);
        }

        protected override void BindProperties()
		{
            rootNamespace = GetConfigProperty(ProjectFileConstants.RootNamespace);
            Control.BindProperties();
		}

		protected override int ApplyChanges()
		{
			Project.SetProjectProperty(ProjectFileConstants.RootNamespace, rootNamespace);
            return VSConstants.S_OK;
		}
    }
}
