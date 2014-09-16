using System.Collections.ObjectModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using Fluor.ProjectSwitcher.Base.Class;
using System.Linq;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class ViewModelApplications :ViewModelBase 
    {
        private ObservableCollection<ProjectSwitcherItem> applicationsCollection;
        public ObservableCollection<ProjectSwitcherItem> ApplicationsCollection
        {
            get
            {
                return applicationsCollection;
            }
            set
            {
                applicationsCollection = value;
                RaisePropertyChanged("ApplicationsCollection");
            }
        }

        private ObservableCollection<MenuItem> applicationContextMenus;
        public ObservableCollection<MenuItem> ApplicationContextMenus
        {
            get
            {
                return applicationContextMenus;
            }
            set
            {
                applicationContextMenus = value;
                RaisePropertyChanged("ApplicationContextMenus");
            }
        }

        //ObservableCollection<ProjectSwitcherItem> filteredApplicationCollection;
        //public ObservableCollection<ProjectSwitcherItem> FilteredApplicationCollection
        //{
        //    get
        //    {
        //        return filteredApplicationCollection;
        //    }
        //    set
        //    {
        //        filteredApplicationCollection = value;
        //        RaisePropertyChanged("FilteredApplicationCollection");
        //    }
        //}

        //Application selectedApplication;
        //public Application SelectedApplication
        //{
        //    get
        //    {
        //        return selectedApplication;
        //    }
        //    set
        //    {
        //        selectedApplication = value;
        //        RaisePropertyChanged("SelectedApplication");

        //        if (check)
        //        {
        //            ChangeSelectedApplication(selectedApplication);
        //        }
        //    }
        //}

        private bool check;

        public ViewModelApplications()
        {
            Messenger.Default.Register<Message.MessagePopulateApplications>(this, UpdateApplicationsCollection);
        }

        private void UpdateApplicationsCollection(Message.MessagePopulateApplications populateApplicationsMessage)
        {
            //check = false;
            ApplicationsCollection = populateApplicationsMessage.ApplicationsCollection;
            //FilteredApplicationCollection = ApplicationsCollection;
            //SelectedApplication = null;
            //check = true;
        }

        //private void ChangeSelectedApplication(Application application)
        //{
        //    if (application.SubItems.Any())
        //    {
        //        check = false;

        //        FilteredApplicationCollection = new ObservableCollection<ProjectSwitcherItem>();
        //        FilteredApplicationCollection.Add(application);

        //        foreach (var subApplication in application.SubItems)
        //        {
        //            FilteredApplicationCollection.Add(subApplication);
        //        }

        //        check = true;
        //    }
        //}

        public void MenuItemClicked(SubApplication selectedApp)
        {
            //TODO Change this to be a property
            foreach (SubApplication subApplication in selectedApp.SubItems)
            {
                subApplication.IsSelected = selectedApp.IsSelected;
            }
        }

        public void DisplayContextMenus(string applicationName)
        {
            // Triggered by a right-click on an application. The treeview does not change the selecteditem when right-clicking
            // so had to write  this routine to change the selected item

            SubApplication subApplication = GetSelectedApplication(applicationName);

            ApplicationContextMenus = new ObservableCollection<MenuItem>();

            // Send a message containing the project name to the main view model. The main view model returns the context
            // menu parameters as listed in the associations section
            Messenger.Default.Send(new NotificationMessageAction<string>(this, subApplication.Name, (contextMenuParameters) =>
            {
                subApplication.CreateContextMenus(contextMenuParameters, ref applicationContextMenus);
            }));
        }

        private SubApplication GetSelectedApplication(string applicationName)
        {
            // Loops through each application & subapplication to find a application with the same name
            // as the application that has been right-clicked.
            // Sets the application to active is changes the treeview's selecteditem property
            foreach (SubApplication subApplication in ApplicationsCollection)
            {
                if (subApplication.Name == applicationName)
                {
                    subApplication.IsActive = true;
                    return subApplication;
                }

                foreach (SubApplication subSubApplication in subApplication.SubItems)
                {
                    if (subSubApplication.Name == applicationName)
                    {
                        subSubApplication.IsActive = true;
                        return subSubApplication;
                    }

                    if (subSubApplication.SubItems.Any())
                    {
                        DisplayContextMenus(subSubApplication.Name);
                    }
                }
            }

            return null;
        }

        //public void SelectApplications(bool isChecked, SPPIDApp selectedApp, MenuItem mi = null)
        //{
        //    //foreach (SPPIDApp sppidApp in ApplicationsCollection)
        //    //{
        //    //    //IF APPLICATION IS ENABLED I.E. NOT A SEPARATOR, HEADER OR HIDDEN ITEM
        //    //    if (sppidApp.IsEnabled == true)
        //    //    {
        //    //        //IF A MENUITEM HAS BEEN PASSED IN THEN EITHER THE "SELECTONLYTHIS" OR "SELECTALLEXCEPTTHIS" BUTTON HAS BEEN CLICKED
        //    //        if (mi != null)
        //    //        {
        //    //            if (selectedApp != null)
        //    //            {
        //    //                if (sppidApp.Name == selectedApp.Name)
        //    //                {
        //    //                    //ASSIGN THE OPPOSITE ISCHECKED VALUE
        //    //                    sppidApp.IsChecked = !isChecked;
        //    //                }
        //    //                else
        //    //                {
        //    //                    sppidApp.IsChecked = isChecked;
        //    //                }
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            sppidApp.IsChecked = isChecked;
        //    //        }
        //    //    }
        //    //}
        //}
    }
}
