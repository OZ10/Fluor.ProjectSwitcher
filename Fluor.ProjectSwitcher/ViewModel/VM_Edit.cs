using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Windows;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class VM_Edit : ViewModelBase 
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

        

        public VM_Edit()
        {
            //Messenger.Default.Register<Message.MessageAddOrEditTile>(this, DisplayTileDetails);
            Messenger.Default.Register<Message.M_AddOrEditTile>(this, ShowProjectDetails);
            Messenger.Default.Register<Message.M_ChangeView>(this, ChangeView);
            Messenger.Default.Register<ObservableCollection<TopApplication>>(this, PopulateApplications);
            Messenger.Default.Register<ObservableCollection<Project>>(this, PopulateExistingAssociations);
            //Messenger.Default.Register<Project>(this, AddNewProject);

            SetViewVisibility(showEditProject:true);
        }

        //private void AddNewProject(Project obj)
        //{
        //    Messenger.Default.Send(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayAddNewTab));
        //}

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

        private void ShowProjectDetails(Message.M_AddOrEditTile msg)
        {
            if (msg.Sender is View.V_Tiles | msg.Sender is App)
            {
                SelectedItem = msg.SelectedTile;

                Messenger.Default.Send(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayAddNewTab));

                // If the selected project only has one assoication, select it
                //if (SelectedItem.Associations.Count == 1)
                //{
                //    SelectedItem.SelectedAssociation = SelectedItem.Associations[0];
                //}
            }
        }

        public void AddNewContextMenu()
        {
            SelectedContextMenu = new Class.ContextMenu();
            SelectedItem.ContextMenuCollection.Add(SelectedContextMenu);
        }

        public void DeleteContextMenu(Class.ContextMenu cm)
        {
            SelectedItem.ContextMenuCollection.Remove(cm);
        }

        public void DeleteAssociation(Class.Association association)
        {
            SelectedItem.Associations.Remove(association);
        }

        public void EditAssociation(bool isNew, Class.Association existingAssociation)
        {
            IsAssociationNew = isNew;

            if (isNew)
            {
                SelectedAssociation = new Association();
                AvailableApplicationsCollection = new ObservableCollection<TopApplication>();

                if (SelectedItem.Associations.Any())
                {
                    foreach (var app in ApplicationsCollection)
                    {
                        if (SelectedItem.Associations.Any((a) => a.Name != app.Name))
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
                Class.Parameter parameter = CreateNewParameter();

                association.Parameters.Add(parameter);

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
            a.Setup(SelectedItem.Name, selectedApplication.Name);
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

        public void OkButton_Clicked()
        {
            if (EditContextMenuVisibility == Visibility.Visible)
            {
                if (SelectedContextMenu.Name == null)
                {
                    SelectedItem.ContextMenuCollection.Remove(SelectedContextMenu);
                }

                SetViewVisibility(true);
            }
            else if (EditAssociatedVisibility == Visibility.Visible)
            {
                if (!SelectedItem.Associations.Contains(SelectedAssociation))
                {
                    // Add new association to the SelectedItem's association collection
                    SelectedItem.Associations.Add(SelectedAssociation);
                }
                
                SetViewVisibility(true);
            }
            else if (SelectExistingAssociationVisibility == Visibility.Visible)
            {
                if (SelectedExistingAssociation != null)
                {
                    SelectedAssociation = new Association();
                    SelectedAssociation.Setup(selectedItem.Name, SelectedExistingAssociation.Name);

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
                Messenger.Default.Send<Message.M_AddOrEditTile>(new Message.M_AddOrEditTile(SelectedItem, this));
            }
            
            //proj.Execute(SelectedItem);
            //Messenger.Default.Send<Message.M_SimpleAction>(new Message.M_SimpleAction(Message.M_SimpleAction.Action.RefreshViews));
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
