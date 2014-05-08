using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluor.ProjectSwitcher.Class;

namespace Fluor.ProjectSwitcher.Class
{
    public class ApplicationBase
    {
        public string Name { get; set; }
        public List<ProjectBase> AssociatedProjects { get; set; }
        public List<SubApplication> SubApplications { get; set; }

        public ApplicationBase()
        {
            AssociatedProjects = new List<ProjectBase>();
            SubApplications = new List<SubApplication>();
        }
    }
}
