using System.Collections.ObjectModel;
using Fluor.ProjectSwitcher.Base.Class;

namespace Fluor.ProjectSwitcher.Message
{
    public class MessagePopulateApplications
    {
        public ObservableCollection<ProjectSwitcherItem> ApplicationsCollection { get; set; }

        public MessagePopulateApplications(ObservableCollection<ProjectSwitcherItem> applicationsCollection)
        {
            ApplicationsCollection = applicationsCollection;
        }
    }
}
