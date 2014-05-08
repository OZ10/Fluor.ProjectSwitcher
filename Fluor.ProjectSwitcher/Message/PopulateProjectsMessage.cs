using System.Collections.ObjectModel;

namespace Fluor.ProjectSwitcher.Message
{
    public class PopulateProjectsMessage
    {
        public ObservableCollection<Class.ProjectBase> ProjectsCollection { get; set; }

        public PopulateProjectsMessage(ObservableCollection<Class.ProjectBase> projectsCollection)
        {
            ProjectsCollection = projectsCollection;
        }
    }
}
