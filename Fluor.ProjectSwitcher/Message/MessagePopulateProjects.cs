using Fluor.ProjectSwitcher.Base.Class;
using System.Collections.ObjectModel;

namespace Fluor.ProjectSwitcher.Message
{
    public class MessagePopulateProjects
    {
        public ObservableCollection<Project> ProjectsCollection { get; set; }

        public MessagePopulateProjects(ObservableCollection<Project> projectsCollection)
        {
            ProjectsCollection = projectsCollection;
        }
    }
}
