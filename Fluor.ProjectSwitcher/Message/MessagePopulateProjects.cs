using System.Collections.ObjectModel;

namespace Fluor.ProjectSwitcher.Message
{
    public class MessagePopulateProjects
    {
        public ObservableCollection<Class.Project> ProjectsCollection { get; set; }

        public MessagePopulateProjects(ObservableCollection<Class.Project> projectsCollection)
        {
            ProjectsCollection = projectsCollection;
        }
    }
}
