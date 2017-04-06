using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluor.ProjectSwitcher.Class;

namespace Fluor.ProjectSwitcher.Message
{
    public class M_ChangeSelectedTile
    {
        public object Sender { get; set; }
        public Project SelectedProject { get; set; }
        public TopApplication SelectedApplication { get; set; }

        public bool ResetBreadcrumb;

        public M_ChangeSelectedTile(Project selectedProject, object sender, bool resetBreadcrumb = false)
        {
            SelectedProject = selectedProject;
            Sender = sender;
            ResetBreadcrumb = resetBreadcrumb;
        }

        public M_ChangeSelectedTile(TopApplication selectedApplication, object sender, bool resetBreadcrumb = false)
        {
            SelectedApplication = selectedApplication;
            Sender = sender;
            ResetBreadcrumb = resetBreadcrumb;
        }
    }
}
