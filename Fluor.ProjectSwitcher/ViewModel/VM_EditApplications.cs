using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class VM_EditApplications : Base
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

        private ObservableCollection<TopApplication> editedApplicationsCollection;
        public ObservableCollection<TopApplication> EditedApplicationsCollection
        {
            get
            {
                return editedApplicationsCollection;
            }
            set
            {
                editedApplicationsCollection = value;
                RaisePropertyChanged("EditedApplicationsCollection");
            }
        }

        bool isEditApplicationsTabSelected;
        public bool IsEditApplicationsTabSelected
        {
            get
            {
                return isEditApplicationsTabSelected;
            }
            set
            {
                isEditApplicationsTabSelected = value;
                RaisePropertyChanged("IsEditApplicationsTabSelected");
            }
        }

        private TopApplication selectedApplication;
        public TopApplication SelectedApplication
        {
            get
            {
                return selectedApplication;
            }
            set
            {
                selectedApplication = value;
                RaisePropertyChanged("SelectedApplication");
            }
        }

        private Visibility editContextMenuVisibility;
        public Visibility EditContextMenuVisibility {
            get { return editContextMenuVisibility; } 
            set
            {
                editContextMenuVisibility = value;
                RaisePropertyChanged("EditContextMenuVisibility");
            }
        }

        private Visibility mainGridVisibility;
        public Visibility MainGridVisibility
        {
            get { return mainGridVisibility; } 
            set
            {
                mainGridVisibility = value;
                RaisePropertyChanged("MainGridVisibility");
            }
        }

        private Class.ContextMenu selectedContextMenu;
        public Class.ContextMenu SelectedContextMenu
        {
            get { return selectedContextMenu; }
            set
            {
                selectedContextMenu = value;

                EditContextMenuVisibility = Visibility.Visible;
                MainGridVisibility = Visibility.Collapsed;

                RaisePropertyChanged("SelectedContextMenu");
            }
        }

        public VM_EditApplications()
        {
            Messenger.Default.Register<ObservableCollection<TopApplication>>(this, PopulateApplications);
            Messenger.Default.Register<Message.M_ChangeView>(this, ChangeView);

            MainGridVisibility = Visibility.Visible;
            EditContextMenuVisibility = Visibility.Collapsed;
        }

        private void PopulateApplications(ObservableCollection<TopApplication> msg)
        {
            ApplicationsCollection = msg;
            EditedApplicationsCollection = ObjectCopier.CloneJson<ObservableCollection<TopApplication>>(ApplicationsCollection);
            //ApplicationsCollection = msg;
        }

        private void ChangeView(Message.M_ChangeView msg)
        {
            if (msg.View == Message.M_ChangeView.ViewToSelect.DisplayEditApplicationsTab)
            {
                IsEditApplicationsTabSelected = true;
            }
            else
            {
                IsEditApplicationsTabSelected = false;
            }
        }

        public void AddNewApplication()
        {
            if (SelectedApplication != null)
            {
                Class.SubApplication subApp = new SubApplication();
                subApp.Setup("New App", "<path>", "<exe>", false, true, true, SelectedApplication.Name);
                SelectedApplication.SubItems.Add(subApp);
                SelectedApplication = subApp;
            }
            else
            {
                Class.TopApplication topApp = new TopApplication();
                topApp.Setup("New Top App", true);
                ApplicationsCollection.Add(topApp);
                SelectedApplication = topApp;
            }
        }

        public void OkButton_Clicked()
        {
            if (MainGridVisibility == Visibility.Visible)
            {
                // Apply changes to the main Applications Collection
                ApplicationsCollection = EditedApplicationsCollection;

                // Toggle the settings class to 'has changed' to trigger a save when the launcher is changed
                Messenger.Default.Send(new Message.M_SettingsHaveBeenChanged(true));

                Messenger.Default.Send(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayTilesTab));
            }
            else
            {
                if (SelectedContextMenu.IsNew)
                {
                    if (SelectedApplication.ContextMenuCollection == null) SelectedApplication.ContextMenuCollection = new ObservableCollection<Class.ContextMenu>();
                    SelectedApplication.ContextMenuCollection.Add(SelectedContextMenu);
                    SelectedContextMenu.IsNew = false;
                }

                SelectedContextMenu.Update();

                //RaisePropertyChanged("SelectedContextMenu");

                MainGridVisibility = Visibility.Visible;
                EditContextMenuVisibility = Visibility.Collapsed;
            }
        }

        public void CancelButton_Clicked()
        {
            if (MainGridVisibility == Visibility.Visible)
            {
                Messenger.Default.Send(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayTilesTab));
            }
            else
            {
                SelectedContextMenu.CancelUpdate();

                MainGridVisibility = Visibility.Visible;
                EditContextMenuVisibility = Visibility.Collapsed;
            }
        }

        public void DeleteButton_Clicked()
        {
            if (SelectedApplication != null)
            {
                if (SelectedApplication.ParentItem == "")
                {
                    ApplicationsCollection.Remove(SelectedApplication);
                }
                else
                {
                    //ApplicationsCollection.Remove(SelectedApplication);
                }
            }
        }

        public void AddNewContextMenu()
        {
            SelectedContextMenu = new Class.ContextMenu();
            SelectedContextMenu.IsNew = true;
            //if (SelectedApplication.ContextMenuCollection == null) SelectedApplication.ContextMenuCollection = new ObservableCollection<Class.ContextMenu>();
            //SelectedApplication.ContextMenuCollection.Add(SelectedContextMenu);
        }

        public void DeleteContextMenu(Class.ContextMenu cm)
        {
            SelectedApplication.ContextMenuCollection.Remove(cm);
        }
    }
}
