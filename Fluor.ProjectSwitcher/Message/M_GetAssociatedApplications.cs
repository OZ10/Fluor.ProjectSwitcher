using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluor.ProjectSwitcher.Message
{
    public class M_GetAssociatedApplications
    {
        public Class.Project SelectedProject;

        public M_GetAssociatedApplications(Class.Project selectedProject)
        {
            SelectedProject = selectedProject;
        }
    }
}
