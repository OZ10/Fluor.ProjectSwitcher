using System.Collections.ObjectModel;
using Fluor.ProjectSwitcher.Base.Class;

namespace Fluor.ProjectSwitcher.Message
{
    public class MessagePopulateApplications
    {
        public ObservableCollection<Application> ApplicationsCollection { get; set; }

        public MessagePopulateApplications(ObservableCollection<Application> applicationsCollection)
        {
            ApplicationsCollection = applicationsCollection;
        }
    }
}
