using System.Collections.ObjectModel;
using Fluor.ProjectSwitcher.Class;

namespace Fluor.ProjectSwitcher.Message
{
    public class PopulateApplicationsMessage
    {
        public ObservableCollection<ApplicationBase> ApplicationsCollection { get; set; }

        public PopulateApplicationsMessage(ObservableCollection<ApplicationBase> applicationsCollection)
        {
            ApplicationsCollection = applicationsCollection;
        }
    }
}
