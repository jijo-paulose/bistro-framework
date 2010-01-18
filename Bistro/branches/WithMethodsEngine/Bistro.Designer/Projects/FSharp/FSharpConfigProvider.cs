using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.ProjectBase;
using System.Runtime.InteropServices;

namespace Bistro.Designer.Projects.FSharp
{
    public class FSharpConfigProvider : ConfigProvider
    {
        [ComVisible(true), CLSCompliant(false)]
        public class FSharpProjectConfig : ProjectConfig
        {
            public FSharpProjectConfig(ProjectNode project, string configuration)
                : base(project, configuration)
            {
            }

            private bool getBool(string projectFileConstant)
            {
                return ("true" == this.GetConfigurationProperty(projectFileConstant, false));
            }

            private void setBool(string projectFileConstant, bool p)
            {
                string propertyValue = p ? "true" : "false";
                this.SetConfigurationProperty(projectFileConstant, propertyValue);
            }

            public string DefineConstants {
                get { return this.GetConfigurationProperty("DefineConstants", false); }
                set { this.SetConfigurationProperty("DefineConstants", value); }
            }
            
            public string DocumentationFile {
                get { return this.GetConfigurationProperty("DocumentationFile", false); }
                set { this.SetConfigurationProperty("DocumentationFile", value); }
            }
            
            public bool Optimize 
            {
                get { return getBool("Optimize"); }
                set { setBool("Optimize", value); }
            }

            public string OtherFlags {
                get { return this.GetConfigurationProperty("OtherFlags", false); }
                set { this.SetConfigurationProperty("OtherFlags", value); }
            }
            //public string OutputPath { get; set; }
            
            public string PlatformTarget {
                get { return this.GetConfigurationProperty("PlatformTarget", false); }
                set { this.SetConfigurationProperty("PlatformTarget", value); }
            }
            
            public bool Tailcalls {
                get { return getBool("Tailcalls"); }
                set { setBool("Tailcalls", value); } 
            }
            
            public string TreatSpecificWarningsAsErrors { 
                get { return this.GetConfigurationProperty("WarningsAsErrors", false); }
                set { this.SetConfigurationProperty("WarningsAsErrors", value); }
            }
            
            public bool TreatWarningsAsErrors {
                get { return getBool("TreatWarningsAsErrors"); }
                set { setBool("TreatWarningsAsErrors", value); }
            }
            
            public int WarningLevel 
            {
                get
                {
                    switch (this.GetConfigurationProperty("WarningLevel", false))
                    {
                        case "0": return 0;
                        case "1": return 1;
                        case "2": return 2;
                        case "3": return 3;
                        case "4": return 4;
                        default:
                            throw new ArgumentException("Invalid WarningLevel value in Project file.");
                    }
                }
                set { this.SetConfigurationProperty("WarningLevel", value.ToString()); }
            }
        }

        ProjectNode project;
        public FSharpConfigProvider(ProjectNode manager)
            : base(manager)
        {
            project = manager;
        }

        protected override ProjectConfig CreateProjectConfiguration(string configName)
        {
            return new FSharpProjectConfig(project, configName);
        }
    }
}
