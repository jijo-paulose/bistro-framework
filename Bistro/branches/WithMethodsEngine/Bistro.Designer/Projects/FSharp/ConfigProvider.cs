using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bistro.Designer.Projects.FSharp
{
    public class ConfigProvider : ProjectBase.ConfigProvider
    {
        ProjectManager manager;
        public ConfigProvider(ProjectManager manager)
            : base(manager)
        {
            this.manager = manager;
        }

        protected override Bistro.Designer.ProjectBase.ProjectConfig CreateProjectConfiguration(string configName)
        {
            return new ProjectConfig(manager, configName);
        }
    }
}
