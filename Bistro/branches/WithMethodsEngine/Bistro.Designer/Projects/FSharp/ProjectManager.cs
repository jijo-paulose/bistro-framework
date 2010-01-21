using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Bistro.Designer.Projects.FSharp.Properties;
using VSLangProj;
using Microsoft.VisualStudio.OLE.Interop;
using System.ComponentModel;

namespace Bistro.Designer.Projects.FSharp
{
    [ComVisible(true)]
    public class ProjectManager : FlavoredProjectBase
    {
        
        /// <summary>
        /// Sets the service provider from which to access the services. 
        /// </summary>
        /// <param name="site">An instance to an Microsoft.VisualStudio.OLE.Interop object</param>
        /// <returns>A success or failure value.</returns>
        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider site)
        {
            serviceProvider = new ServiceProvider(site);
            return VSConstants.S_OK;
        }

        const string debug_page_guid = "";

        protected override int GetProperty(uint itemId, int propId, out object property)
        {
            int result = base.GetProperty(itemId, propId, out property);
            if (itemId == VSConstants.VSITEMID_ROOT)
            {
                switch ((__VSHPROPID2)propId)
                {
                    case __VSHPROPID2.VSHPROPID_CfgPropertyPagesCLSIDList:
                        //Remove the Debug page
                        property = property.ToString().Split(';')
                            .Aggregate("", (a, next) => next == debug_page_guid ? a : a + ';' + next).Substring(1);
                        return VSConstants.S_OK;
                    case __VSHPROPID2.VSHPROPID_PropertyPagesCLSIDList:
                        //Add the Deployment property page.
                        property += ';' + typeof(CompileOrderPage).GUID.ToString("B");
                        return VSConstants.S_OK;
                    //case __VSHPROPID2.VSHPROPID_BrowseObjectCATID:
                    //    property = null;
                    //    return VSConstants.DISP_E_MEMBERNOTFOUND;
                    default:
                        break;
                }
                //switch ((__VSHPROPID)propId)
                //{
                //    case __VSHPROPID.VSHPROPID_BrowseObject:
                //        //if (propWrapper == null)
                //        //    propWrapper = new ProjectPropertiesWrapper(property);
                //        //property = propWrapper;
                //        return result;
                //    default:
                //        break;
                //}
            }
            return result;
        }

    }
}
