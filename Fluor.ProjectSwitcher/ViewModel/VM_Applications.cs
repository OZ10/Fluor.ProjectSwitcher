using System.Collections.ObjectModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using Fluor.ProjectSwitcher.Class;
using System.Linq;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class VM_Applications : ViewModelBase
    {
        private ObservableCollection<TopApplication> applicationsCollection;
        public ObservableCollection<TopApplication> ApplicationsCollection
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

        private ObservableCollection<TopApplication> activeApplicationCollection;
        public ObservableCollection<TopApplication> ActiveApplicationCollection
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

        bool isApplicationsTabSelected;
        public bool IsApplicationsTabSelected
        {
            get
            {
                return isApplicationsTabSelected;
            }
            set
            {
                isApplicationsTabSelected = value;
                RaisePropertyChanged("IsApplicationsTabSelected");
            }
        }

        public TopApplication SelectedApplication { get; set; }

        public VM_Applications()
        {
            Messenger.Default.Register<GenericMessage<TopApplication>>(this, UpdateApplicationsCollection);
            Messenger.Default.Register<GenericMessage<MenuItem>>(this, SelectApplications);
            Messenger.Default.Register<ObservableCollection<TopApplication>>(this, PopulateApplications);
            Messenger.Default.Register<Message.M_GetAssociatedApplications>(this, GetAssociatedApplications);
            Messenger.Default.Register<Message.M_ChangeView>(this, ChangeView);
        }

        private void GetAssociatedApplications(Message.M_GetAssociatedApplications msg)
        {
            // Clear SelectedProject's applications collection 
            msg.SelectedProject.Applications = new ObservableCollection<TopApplication>();

            // Get all the associations associated with the selected item
            foreach (Association association in msg.SelectedProject.Associations) //.Where(ass => ass.ProjectName == SelectedTile.Name))
            {
                foreach (TopApplication application in ApplicationsCollection.Where(app => app.Name == association.Name))
                {
                    msg.SelectedProject.Applications.Add(application);
                }
            }
        }

        /// <summary>
        /// Reads the application details from the xml file and populates the applications collection.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        private void PopulateApplications(ObservableCollection<TopApplication> msg)
        {
            ApplicationsCollection = msg;
        }

        private void UpdateApplicationsCollection(GenericMessage<TopApplication> message)
        {
            DisplayApplicationsTab();

            SelectedApplication = message.Content;

            ActiveApplicationCollection = new ObservableCollection<TopApplication>(message.Content.SubItems);
        }

        public void DisplayContextMenus(TextBlock tb)
        {
            // Triggered by a right-click on an application.
            SubApplication subApplication = (SubApplication)tb.DataContext;  //GetSelectedApplication(applicationName);

            //ApplicationContextMenus = new ObservableCollection<MenuItem>();

            // Send a message containing the project name to the main view model. The main view model returns the context
            // menu parameters as listed in the associations section
            Messenger.Default.Send(new NotificationMessageAction<string>(subApplication, subApplication.Name, (contextMenuParameters) =>
            {
                //subApplication.CreateContextMenus(contextMenuParameters);
            }));

            tb.ContextMenu.ItemsSource = subApplication.ContextMenuCollection;
        }

        public void SelectApplications(GenericMessage<MenuItem> msg)
        {
            SwitcherItem selectedApp = (SwitcherItem)msg.Content.DataContext;
            string selectedMenuItem = msg.Content.Header.ToString();

            foreach (SubApplication subApp in SelectedApplication.SubItems.Where(sa => sa.Name == selectedApp.ParentItem))
            {
                //TODO This only goes one level deep on subapps. Should recurse through all subitems.
                foreach (SubApplication subApp2 in subApp.SubItems) //.Where(sa => sa.Name == selectedApp.ParentItem))
                {
                    subApp2.IsSelected = false;

                    switch (selectedMenuItem)
                    {
                        case "select all":
                            subApp2.IsSelected = true;
                            break;

                        //case "select none":
                        //    subApp.IsSelected = false;
                        //    break;

                        case "select only this":
                            if (subApp2 == selectedApp)
                            {
                                subApp2.IsSelected = true;
                            }
                            break;

                        case "select all except this":
                            if (subApp2 != selectedApp)
                            {
                                subApp2.IsSelected = true;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void ChangeView(Message.M_ChangeView msg)
        {
            if (msg.View == Message.M_ChangeView.ViewToSelect.DisplayApplicationsTab)
            {
                IsApplicationsTabSelected = true;
            }
            else
            {
                IsApplicationsTabSelected = false;
            }
        }

        private void DisplayApplicationsTab()
        {
            Messenger.Default.Send<Message.M_ChangeView>(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayApplicationsTab));
        }
    }
}