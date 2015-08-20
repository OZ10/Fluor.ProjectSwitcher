using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class VM_EditTile : ViewModelBase
    {
        #region Collections
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

        private ObservableCollection<TopApplication> availableApplicationsCollection;
        public ObservableCollection<TopApplication> AvailableApplicationsCollection
        {
            get
            {
                return availableApplicationsCollection;
            }
            set
            {
                availableApplicationsCollection = value;
                RaisePropertyChanged("AvailableApplicationsCollection");
            }
        }

        private ObservableCollection<Association> existingAssociationCollection;
        public ObservableCollection<Association> ExistingAssociationCollection
        {
            get
            {
                return existingAssociationCollection;
            }
            set
            {
                existingAssociationCollection = value;
                RaisePropertyChanged("ExistingAssociationCollection");
            }
        }

        private ObservableCollection<Association> availableExistingAssociationCollection;
        public ObservableCollection<Association> AvailableExistingAssociationCollection
        {
            get
            {
                return availableExistingAssociationCollection;
            }
            set
            {
                availableExistingAssociationCollection = value;
                RaisePropertyChanged("AvailableExistingAssociationCollection");
            }
        }
        #endregion
        
        #region bools

        bool isAddNewTabSelected;
        public bool IsAddNewTabSelected
        {
            get { return isAddNewTabSelected; }
            set
            {
                isAddNewTabSelected = value;
                RaisePropertyChanged("IsAddNewTabSelected");
            }
        }

        bool isAssociationNew;
        public bool IsAssociationNew
        {
            get { return isAssociationNew; }
            set
            {
                isAssociationNew = value;
                RaisePropertyChanged("IsAssociationNew");
            }
        }

        #endregion

        #region Selected

        private Project selectedItem;
        public Project SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        private Project editedItem;
        public Project EditedItem
        {
            get
            {
                return editedItem;
            }
            set
            {
                editedItem = value;
                RaisePropertyChanged("EditedItem");
            }
        }

        private Class.ContextMenu selectedContextMenu;
        public Class.ContextMenu SelectedContextMenu
        {
            get { return selectedContextMenu; }
            set
            {
                selectedContextMenu = value;

                SetViewVisibility(showEditContextMenu:true);

                RaisePropertyChanged("SelectedContextMenu");
            }
        }

        private Association selectedAssociation;
        public Association SelectedAssociation
        {
            get
            {
                return selectedAssociation;
            }
            set
            {
                selectedAssociation = value;
                //SetViewVisibility(showEditAssociation:true);
                RaisePropertyChanged("SelectedAssociation");
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

                if (SelectedApplication != null)
                {
                    SelectedAssociationChanged(SelectedApplication);
                }

                RaisePropertyChanged("SelectedApplication");
            }
        }

        private Association selectedExistingAssociation;
        public Association SelectedExistingAssociation
        {
            get
            {
                return selectedExistingAssociation;
            }
            set
            {
                selectedExistingAssociation = value;

                //if (selectedExistingAssociation != null)
                //{
                //    SelectedAssociation = selectedExistingAssociation;
                //}

                RaisePropertyChanged("SelectedExistingAssociation");
            }
        }

        #endregion       

        #region Visibilities

        private Visibility editContextMenuVisibility;
        public Visibility EditContextMenuVisibility
        {
            get { return editContextMenuVisibility; }
            set
            {
                editContextMenuVisibility = value;
                RaisePropertyChanged("EditContextMenuVisibility");
            }
        }

        private Visibility editProjectVisibility;
        public Visibility EditProjectVisibility
        {
            get { return editProjectVisibility; }
            set
            {
                editProjectVisibility = value;
                RaisePropertyChanged("EditProjectVisibility");
            }
        }

        private Visibility editAssociatedVisibility;
        public Visibility EditAssociatedVisibility
        {
            get { return editAssociatedVisibility; }
            set
            {
                editAssociatedVisibility = value;
                RaisePropertyChanged("EditAssociatedVisibility");
            }
        }

        private Visibility selectExistingAssociationVisibility;
        public Visibility SelectExistingAssociationVisibility
        {
            get { return selectExistingAssociationVisibility; }
            set
            {
                selectExistingAssociationVisibility = value;
                RaisePropertyChanged("SelectExistingAssociationVisibility");
            }
        }

	    #endregion

        public VM_EditTile()
        {
            Messenger.Default.Register<Message.M_EditTile>(this, ShowProjectDetails);
            Messenger.Default.Register<Message.M_ChangeView>(this, ChangeView);
            Messenger.Default.Register<ObservableCollection<TopApplication>>(this, PopulateApplications);
            Messenger.Default.Register<ObservableCollection<Project>>(this, PopulateExistingAssociations);

            SetViewVisibility(showEditProject:true);
        }

        private void PopulateApplications(ObservableCollection<TopApplication> msg)
        {
            ApplicationsCollection = msg;
        }

        private void PopulateExistingAssociations(ObservableCollection<Project> msg)
        {
            ExistingAssociationCollection = new ObservableCollection<Association>();

            foreach (var project in msg)
            {
                foreach (var association in project.Associations)
                {
                    ExistingAssociationCollection.Add(association);
                }
            }
        }

        private void SetViewVisibility(bool showEditProject = false, bool showEditContextMenu = false, bool showEditAssociation = false, bool showExistingAssociationVisibility = false)
        {
            EditProjectVisibility = BoolToVisibility(showEditProject);
            EditContextMenuVisibility = BoolToVisibility(showEditContextMenu);
            EditAssociatedVisibility = BoolToVisibility(showEditAssociation);
            SelectExistingAssociationVisibility = BoolToVisibility(showExistingAssociationVisibility);
        }

        private Visibility BoolToVisibility(bool b)
        {
            switch (b)
            {
                case true:
                    return Visibility.Visible;                   
                default:
                    return Visibility.Hidden;
            }
        }

        private void ShowProjectDetails(Message.M_EditTile msg)
        {
            if (msg.Sender is View.V_Tiles | msg.Sender is App)
            {
                SelectedItem = msg.SelectedTile;

                EditedItem = ObjectCopier.CloneJson<Project>(SelectedItem);

                Messenger.Default.Send(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayAddNewTab));
            }
        }

        public void AddNewContextMenu()
        {
            SelectedContextMenu = new Class.ContextMenu();
            if (EditedItem.CombinedContextMenuCollection == null) EditedItem.CombinedContextMenuCollection = new ObservableCollection<Class.ContextMenu>();
            EditedItem.CombinedContextMenuCollection.Add(SelectedContextMenu);
        }

        public void DeleteContextMenu(Class.ContextMenu cm)
        {
            EditedItem.CombinedContextMenuCollection.Remove(cm);
        }

        public void DeleteAssociation(Class.Association association)
        {
            EditedItem.Associations.Remove(association);
        }

        public void EditAssociation(bool isNew, Class.Association existingAssociation)
        {
            IsAssociationNew = isNew;

            if (isNew)
            {
                SelectedAssociation = new Association();
                AvailableApplicationsCollection = new ObservableCollection<TopApplication>();

                if (EditedItem.Associations.Any())
                {
                    foreach (var app in ApplicationsCollection)
                    {
                        if (EditedItem.Associations.Any((a) => a.Name != app.Name))
                        {
                            AvailableApplicationsCollection.Add(app);
                        }
                    }
                }
                else
                {
                    foreach (var a in ApplicationsCollection)
                    {
                        AvailableApplicationsCollection.Add(a);
                    }
                }
            }
            else
            {
                SelectedAssociation = existingAssociation;
                AvailableApplicationsCollection = new ObservableCollection<TopApplication>();
                SelectedApplication = ApplicationsCollection.First((a) => a.Name == existingAssociation.Name);
                AvailableApplicationsCollection.Add(SelectedApplication);
            }

            SetViewVisibility(showEditAssociation: true);
        }

        public void SelectedAssociationChanged(Class.TopApplication selectedApplication)
        {
            if (IsAssociationNew)
            {
                // Create new association and add a new parameter
                Class.Association association = CreateNewAssociation(selectedApplication);
                //Class.Parameter parameter = CreateNewParameter();

                //association.Parameters.Add(parameter);

                //// Add new association to the SelectedItem's association collection
                //SelectedItem.Associations.Add(association);

                // Make the new assoication the SelectedAssociation
                SelectedAssociation = association;
            }

            AvailableExistingAssociationCollection = new ObservableCollection<Association>();
            foreach (var association in ExistingAssociationCollection)
            {
                if (association.Name == selectedApplication.Name)
                {
                    AvailableExistingAssociationCollection.Add(association);
                }
            }
        }

        private static Parameter CreateNewParameter()
        {
            Class.Parameter p = new Parameter();
            p.Setup(Parameter.ParameterTypeEnum.INI, "", "");
            return p;
        }

        private Association CreateNewAssociation(Class.TopApplication selectedApplication)
        {
            Class.Association a = new Association();
            a.Setup(EditedItem.Name, selectedApplication.Name);
            return a;
        }

        public void CopyParametersFromExisingAssociation()
        {
            SetViewVisibility(showExistingAssociationVisibility: true);
            SelectedAssociation = null;
        }

        public void AddNewParameter()
        {
            Class.Parameter p = new Parameter();
            p.Setup(Parameter.ParameterTypeEnum.INI, "", "");

            SelectedAssociation.Parameters.Add(p);
        }

        public void AddNewAssociationContextMenu()
        {
            Class.ContextMenu cm = new Class.ContextMenu();
            cm.Setup(Class.ContextMenu.ContextMenuTypeEnum.PATH, "", "");

            if (SelectedAssociation.ContextMenuCollection == null) SelectedAssociation.ContextMenuCollection = new ObservableCollection<Class.ContextMenu>();
            SelectedAssociation.ContextMenuCollection.Add(cm);
        }

        public void OkButton_Clicked()
        {
            if (EditContextMenuVisibility == Visibility.Visible)
            {
                if (SelectedContextMenu.Name == null)
                {
                    EditedItem.CombinedContextMenuCollection.Remove(SelectedContextMenu);
                }

                SetViewVisibility(true);
            }
            else if (EditAssociatedVisibility == Visibility.Visible)
            {
                if (!EditedItem.Associations.Contains(SelectedAssociation))
                {
                    // Add new association to the EditedItem's association collection
                    EditedItem.Associations.Add(SelectedAssociation);
                }
                
                SetViewVisibility(true);
            }
            else if (SelectExistingAssociationVisibility == Visibility.Visible)
            {
                if (SelectedExistingAssociation != null)
                {
                    SelectedAssociation = new Association();
                    SelectedAssociation.Setup(EditedItem.Name, SelectedExistingAssociation.Name);

                    foreach (var parameter in SelectedExistingAssociation.Parameters)
                    {
                        Parameter newParameter = new Parameter();
                        newParameter.Setup(parameter.Type, parameter.Value, parameter.Path);
                        SelectedAssociation.Parameters.Add(newParameter);
                    }

                    foreach (var cm in SelectedExistingAssociation.ContextMenuCollection)
                    {
                        Class.ContextMenu newContextMenu = new Class.ContextMenu();
                        newContextMenu.Setup(cm.Type, cm.Value, cm.Name);
                        SelectedAssociation.AddContextMenu(cm);
                    }
                }
                
                SetViewVisibility(showEditAssociation:true);
            }
            else
            {
                SelectedItem.Name = EditedItem.Name;
                SelectedItem.CombinedContextMenuCollection = EditedItem.CombinedContextMenuCollection;
                SelectedItem.Applications = editedItem.Applications;
                SelectedItem.Associations = EditedItem.Associations;
                SelectedItem.MiscText = EditedItem.MiscText;
                Messenger.Default.Send<Message.M_EditTile>(new Message.M_EditTile(SelectedItem, this));
            }
            
            //proj.Execute(EditedItem);
            //Messenger.Default.Send<Message.M_SimpleAction>(new Message.M_SimpleAction(Message.M_SimpleAction.Action.RefreshViews));
        }

        public void CancelButton_Clicked()
        {
            // Got back to the tile display if the main edit project view is displayed
            if (EditProjectVisibility == Visibility.Visible)
            {
                Messenger.Default.Send(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayTilesTab));
            }

            SetViewVisibility(showEditProject: true);
        }

        public void DeleteProjectButton_Clicked()
        {
            SelectedItem.IsDeleted = true;
            Messenger.Default.Send<Message.M_EditTile>(new Message.M_EditTile(SelectedItem, this));
        }

        private void ChangeView(Message.M_ChangeView msg)
        {
            if (msg.View == Message.M_ChangeView.ViewToSelect.DisplayAddNewTab)
            {
                IsAddNewTabSelected = true;
            }
            else
            {
                IsAddNewTabSelected = false;
            }
        }
    }
}
