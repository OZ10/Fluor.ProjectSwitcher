using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluor.ProjectSwitcher.Class
{
    public class Association
    {
        public string ProjectName { get; set; }
        public string ApplicationName { get; set; }
        public string Parameters { get; set; }
        public string ContextMenus { get; set; }

        public Association(string projectName, string applicationName, string projectParameters, string contextMenus)
        {
            ProjectName = projectName;
            ApplicationName = applicationName;
            Parameters = projectParameters;
            ContextMenus = contextMenus;
        }
    }
}
