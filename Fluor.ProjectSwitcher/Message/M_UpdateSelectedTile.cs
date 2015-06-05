using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluor.ProjectSwitcher.Class;

namespace Fluor.ProjectSwitcher.Message
{
    public class M_UpdateSelectedTile
    {
        public object Sender { get; set; }
        public Project SelectedProject { get; set; }
        public TopApplication SelectedApplication { get; set; }

        public M_UpdateSelectedTile(Project selectedProject, object sender)
        {
            SelectedProject = selectedProject;
            Sender = sender;
        }

        public M_UpdateSelectedTile(TopApplication selectedApplication, object sender)
        {
            SelectedApplication = selectedApplication;
            Sender = sender;
        }
    }
}
