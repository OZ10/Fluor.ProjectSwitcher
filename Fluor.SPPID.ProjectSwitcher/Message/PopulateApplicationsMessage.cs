using System.Collections.ObjectModel;

namespace Fluor.SPPID.ProjectSwitcher.Message
{
    public class PopulateApplicationsMessage
    {
        public ObservableCollection<SPPIDApp> ApplicationsCollection { get; set; }

        public PopulateApplicationsMessage(ObservableCollection<SPPIDApp> applicationsCollection)
        {
            ApplicationsCollection = applicationsCollection;
        }
    }
}
