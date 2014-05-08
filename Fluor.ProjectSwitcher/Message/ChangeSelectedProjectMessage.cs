
using Fluor.ProjectSwitcher.Class;
namespace Fluor.ProjectSwitcher.Message
{
    public class ChangeSelectedProjectMessage
    {
        public ProjectBase SelectedProject { get; set; }

        public ChangeSelectedProjectMessage(ProjectBase selectedProject)
        {
            SelectedProject = selectedProject;
        }
    }
}
