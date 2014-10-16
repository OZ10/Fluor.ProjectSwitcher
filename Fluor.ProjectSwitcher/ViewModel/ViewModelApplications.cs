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

        public ViewModelApplications()
        {
            Messenger.Default.Register<GenericMessage<Fluor.ProjectSwitcher.Base.Class.Application>>(this, UpdateApplicationsCollection);
            Messenger.Default.Register<GenericMessage<MenuItem>>(this, SelectApplications);
            Messenger.Default.Register<GenericMessage<TextBlock>>(this, DisplayContextMenus);
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
        }

        public void DisplayContextMenus(GenericMessage<TextBlock> msg)
        {
            // Triggered by a right-click on an application.

            TextBlock tb = (TextBlock)msg.Content;

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

        public void SelectApplications(GenericMessage<MenuItem> msg)
        {
            ProjectSwitcherItem selectedApp = (ProjectSwitcherItem)msg.Content.DataContext;
            string selectedMenuItem = msg.Content.Header.ToString();

            foreach (Fluor.ProjectSwitcher.Base.Class.Application application in ApplicationsCollection.Where(sa => sa == selectedApp.ParentItem))
            {
                foreach (SubApplication subApp in application.SubItems)
                {
                    subApp.IsSelected = false;

                    switch (selectedMenuItem)
                    {
                        case "select all":
                            subApp.IsSelected = true;
                            break;

                        //case "select none":
                        //    subApp.IsSelected = false;
                        //    break;

                        case "select only this":
                            if (subApp == selectedApp)
                            {
                                subApp.IsSelected = true;
                            }
                            break;

                        case "select all except this":
                            if (subApp != selectedApp)
                            {
                                subApp.IsSelected = true;
                            }
                            break;

                        default:
                            break;
                    }
                    
                }
            }
        }
    }
}
