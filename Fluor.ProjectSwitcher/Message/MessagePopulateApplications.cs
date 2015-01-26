using System.Collections.ObjectModel;
using Fluor.ProjectSwitcher.Class;

namespace Fluor.ProjectSwitcher.Message
{
    public class MessagePopulateApplications
    {
        public ObservableCollection<SwitcherItem> ApplicationsCollection { get; set; }

        public MessagePopulateApplications(ObservableCollection<SwitcherItem> applicationsCollection)
        {
            ApplicationsCollection = applicationsCollection;
        }
    }
}
