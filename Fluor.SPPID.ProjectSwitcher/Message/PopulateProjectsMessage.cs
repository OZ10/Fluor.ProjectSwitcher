using System.Collections.ObjectModel;

namespace Fluor.SPPID.ProjectSwitcher.Message
{
    public class PopulateProjectsMessage
    {
        public ObservableCollection<SPPIDProject> ProjectsCollection { get; set; }

        public PopulateProjectsMessage(ObservableCollection<SPPIDProject> projectsCollection)
        {
            ProjectsCollection = projectsCollection;
        }
    }
}
