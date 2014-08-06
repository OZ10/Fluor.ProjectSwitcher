
using Fluor.ProjectSwitcher.Base.Class;
namespace Fluor.ProjectSwitcher.Message
{
    public class MessageChangeSelectedProject
    {
        public Project SelectedProject { get; set; }

        public MessageChangeSelectedProject(Project selectedProject)
        {
            SelectedProject = selectedProject;
        }
    }
}
