using System;
using System.Linq;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Bistro.Designer.Projects.FSharp.Properties;
using Microsoft.VisualStudio.OLE.Interop;

using IOLEServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.Collections.Generic;

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

        const string debug_page_guid = "{9CFBEB2A-6824-43e2-BD3B-B112FEBC3772}";

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
                            .Aggregate("", (a, next) => next.Equals(debug_page_guid, StringComparison.OrdinalIgnoreCase) ? a : a + ';' + next).Substring(1);
                        return VSConstants.S_OK;
                    case __VSHPROPID2.VSHPROPID_PropertyPagesCLSIDList:
                        //Add the CompileOrder property page.
                        var properties = new List<string>(property.ToString().Split(';'));
                        properties.Insert(1, typeof(CompileOrderPage).GUID.ToString("B"));
                        property = properties.Aggregate("", (a, next) => a + ';' + next).Substring(1);
                        return VSConstants.S_OK;
                    default:
                        break;
                }
            }
            return result;
        }

    }
}
