using System.Collections.ObjectModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using Fluor.ProjectSwitcher.Base.Class;
using System.Linq;
using MahApps.Metro.Controls;
using System.Windows;

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

        private ObservableCollection<ListBox> activeApplicationCollection;
        public ObservableCollection<ListBox> ActiveApplicationCollection
        {
            get
            {
                return activeApplicationCollection;
            }
            set
            {
                activeApplicationCollection = value;
                RaisePropertyChanged("ActiveApplicationCollection");
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
            Messenger.Default.Register<GenericMessage<Fluor.ProjectSwitcher.Base.Class.Application>>(this, UpdateApplicationsCollection);
        }

        private void UpdateApplicationsCollection(GenericMessage<Fluor.ProjectSwitcher.Base.Class.Application> message)
        {
            //check = false;
            ApplicationsCollection = message.Content.SubItems;

            ActiveApplicationCollection = new ObservableCollection<ListBox>();

            foreach (Fluor.ProjectSwitcher.Base.Class.Application application in ApplicationsCollection)
            {
                ListBox lb = new ListBox();
                lb.Template = (ControlTemplate)System.Windows.Application.Current.Resources["ApplicationListTemplate"];
                lb.DataContext = application;

                ActiveApplicationCollection.Add(lb);
            }
           
            //FilteredApplicationCollection = ApplicationsCollection;
            //SelectedApplication = null;
            //check = true;
        }

        private void Application_Clicked(object sender, RoutedEventArgs e)
        {
            //Tile tile = (Tile)sender;
            //Project project = (Project)tile.DataContext;

            ////SelectedProject = project;

            //if (project.SubItems.Any())
            //{
            //    ActiveProjectTileCollection = new ObservableCollection<Tile>();

            //    foreach (ProjectSwitcherItem subProject in project.SubItems)
            //    {
            //        Tile tvi = CreateTile(subProject);

            //        ActiveProjectTileCollection.Add(tvi);
            //    }
            //}

            //SelectedProject = project;
            //Messenger.Default.Send<GenericMessage<Project>>(new GenericMessage<Project>(project));
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

        public void DisplayContextMenus(object sender)
        {
            // Triggered by a right-click on an application. The treeview does not change the selecteditem when right-clicking
            // so had to write  this routine to change the selected item

            TextBlock tb = (TextBlock)sender;

            SubApplication subApplication = (SubApplication)tb.DataContext;  //GetSelectedApplication(applicationName);

            ApplicationContextMenus = new ObservableCollection<MenuItem>();

            // Send a message containing the project name to the main view model. The main view model returns the context
            // menu parameters as listed in the associations section
            Messenger.Default.Send(new NotificationMessageAction<string>(subApplication, subApplication.Name, (contextMenuParameters) =>
            {
                subApplication.CreateContextMenus(contextMenuParameters, ref applicationContextMenus);
            }));

            tb.ContextMenu.ItemsSource = ApplicationContextMenus;
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
