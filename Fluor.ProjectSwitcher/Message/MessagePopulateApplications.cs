using System.Collections.ObjectModel;
using Fluor.ProjectSwitcher.Class;

namespace Fluor.ProjectSwitcher.Message
{
    public class MessagePopulateApplications
    {
        public ObservableCollection<ApplicationBase> ApplicationsCollection { get; set; }

        public MessagePopulateApplications(ObservableCollection<ApplicationBase> applicationsCollection)
        {
            ApplicationsCollection = applicationsCollection;
        }
    }
}
