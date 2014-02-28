
namespace Fluor.SPPID.ProjectSwitcher.Message
{
    public class ChangeSelectedProjectMessage
    {
        public SPPIDProject SelectedProject { get; set; }

        public ChangeSelectedProjectMessage(SPPIDProject selectedProject)
        {
            SelectedProject = selectedProject;
        }
    }
}
