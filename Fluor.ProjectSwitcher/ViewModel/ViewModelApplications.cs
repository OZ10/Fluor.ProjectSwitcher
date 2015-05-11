using System.Collections.ObjectModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using Fluor.ProjectSwitcher.Class;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class ViewModelApplications : ViewModelBase
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

        public ViewModelApplications()
        {
            Messenger.Default.Register<GenericMessage<TopApplication>>(this, UpdateApplicationsCollection);
            Messenger.Default.Register<GenericMessage<MenuItem>>(this, SelectApplications);
            Messenger.Default.Register<GenericMessage<TextBlock>>(this, DisplayContextMenus);
            Messenger.Default.Register<ObservableCollection<TopApplication>>(this, PopulateApplications);
            Messenger.Default.Register<Message.M_GetAssociatedApplications>(this, GetAssociatedApplications);
            Messenger.Default.Register<Message.M_ChangeView>(this, ChangeView);
        }

        private void GetAssociatedApplications(Message.M_GetAssociatedApplications msg)
        {
            // Get all the associations associated with the selected item
            foreach (Association association in msg.SelectedProject.Associations) //.Where(ass => ass.ProjectName == SelectedTile.Name))
            {
                foreach (TopApplication application in ApplicationsCollection.Where(app => app.Name == association.ApplicationName))
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
            //check = false;
            // ApplicationsCollection = message.Content.SubItems;

            ActiveApplicationCollection = new ObservableCollection<ListBox>();

            foreach (TopApplication application in message.Content.SubItems)
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

            foreach (TopApplication application in ApplicationsCollection.Where(sa => sa == selectedApp.ParentItem))
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
    }
}