using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using Fluor.ProjectSwitcher.Class;
using System.Windows;
using System;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class VM_Tiles : ViewModelBase
    {
        private ObservableCollection<Project> projectsCollection;
        public ObservableCollection<Project> ProjectsCollection
        {
            get
            {
                return projectsCollection;
            }
            set
            {
                projectsCollection = value;
                RaisePropertyChanged("ProjectsCollection");
            }
        }

        private ObservableCollection<SwitcherItem> activeTileCollection;
        public ObservableCollection<SwitcherItem> ActiveTileCollection
        {
            get
            {
                return activeTileCollection;
            }
            set
            {
                activeTileCollection = value;
                RaisePropertyChanged("ActiveTileCollection");
            }
        }

        private Project selectedTile;
        public Project SelectedTile
        {
            get
            {
                return selectedTile;
            }
            set
            {
                selectedTile = value;
                RaisePropertyChanged("SelectedTile");
            }
        }

        bool isTileTabSelected;
        public bool IsTileTabSelected
        {
            get
            {
                return isTileTabSelected;
            }
            set
            {
                isTileTabSelected = value;

                RaisePropertyChanged("IsTileTabSelected");
            }
        }

        double addNewButtonHeight;
        public double AddNewButtonHeight
        {
            get
            {
                return addNewButtonHeight;
            }
            set
            {
                addNewButtonHeight = value;

                RaisePropertyChanged("AddNewButtonHeight");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VM_Tiles"/> class.
        /// </summary>
        public VM_Tiles()
        {
            Messenger.Default.Register<GenericMessage<SwitcherItem>>(this, GoBackToParent);
            Messenger.Default.Register<ObservableCollection<Project>>(this, PopulateProjects);
            Messenger.Default.Register<Message.M_EditTile>(this, SaveChangesToProject);
            Messenger.Default.Register<Message.M_ChangeView>(this, ChangeView);
            Messenger.Default.Register<Message.M_UpdateSelectedTile>(this, TileClicked);
            Messenger.Default.Register<Message.M_SimpleAction>(this, ChangeActiveProject);
        }

        /// <summary>
        /// Reads the project details from the xml file and populates the projects listview.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        private void PopulateProjects(ObservableCollection<Project> msg)
        {
            try
            {
                SortProjectsByPosition(ref msg);

                ProjectsCollection = msg; //new ObservableCollection<Project>(msg.OrderBy((proj) => proj.Position));

                CreateTiles();

                GetPreselectedProject();

                CombineContextMenus(ProjectsCollection);

                // Select the project if it's been pre-selected or just show the tiles tab
                if (SelectedTile != null)
                {
                    Messenger.Default.Send(new Message.M_UpdateSelectedTile(SelectedTile, this));
                }
                else
                {
                    Messenger.Default.Send<Message.M_ChangeView>(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayTilesTab));
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Errooooooooorrrrrr");
                throw;
            }
        }

        private void SortProjectsByPosition(ref ObservableCollection<Project> projectCollection)
        {
            projectCollection = new ObservableCollection<Project>(projectCollection.OrderBy((proj) => proj.Position));

            foreach (var proj in projectCollection)
            {
                // Can't pass a property as ref so create a local variable, sort it and then re-assign it to the property
                ObservableCollection<Project> subProjs = proj.SubItems;
                SortProjectsByPosition(ref subProjs);
                proj.SubItems = subProjs;
            }
        }

        private void CombineContextMenus(ObservableCollection<Project> collection)
        {
            // Combines all context menus from child to parent
            foreach (var project in collection)
            {
                project.CombineContextMenus();

                if (project.SubItems.Any())
                {
                    CombineContextMenus(project.SubItems);
                }
            }
        }

        private void GetPreselectedProject()
        {
            // Send a message to the App.xaml.cs to retrieve the pre-Selected Project name (as 
            // selected from the jumplist menu)
            Messenger.Default.Send(new NotificationMessageAction<string>("", (preSelectedProject) =>
            {
                if (preSelectedProject != "")
                {
                    // Search through all the projects to find the one with the same name
                    foreach (var proj in ProjectsCollection)
                    {
                        SelectedTile = proj.FindSubProjectByName(proj, preSelectedProject);

                        if (SelectedTile != null) { break; }
                    }
                }
            }));
        }

        /// <summary>
        /// reates a tile for each project.
        /// </summary>
        /// <param name="msg">Message containing a collection of projects.</param>
        private void CreateTiles()
        {
            ActiveTileCollection = new ObservableCollection<SwitcherItem>(ProjectsCollection);

            AddNewButtonHeight = SetAddNewButtonHeight();
        }

        private double SetAddNewButtonHeight()
        {
            if (ProjectsCollection.Any())
            {
                return 15;
            }
            else
            {
                return 50;
            }
        }

        /// <summary>
        /// Updates the applications collection and creates a tile for each application.
        /// </summary>
        /// <param name="msg">Message containing a collection of applications.</param>
        private void DisplayApplicationsAsTiles()
        {
            ActiveTileCollection = new ObservableCollection<SwitcherItem>(SelectedTile.Applications);
        }

        private void TileClicked(Message.M_UpdateSelectedTile msg)
        {
            // Try to cast the item as a Project
            Project project = msg.SelectedProject as Project;

            if (project != null)
            {
                if (project.SubItems.Any())
                {
                    // Reset the active tile collection and add all subitems of the selected tile
                    ActiveTileCollection = new ObservableCollection<SwitcherItem>(project.SubItems);
                }

                SelectedTile = project;

                GetAssociatedApplications();
            }
            else
            {
                // Selected tile must be an application. Send a message to the main view model containing the selected application
                Messenger.Default.Send(new GenericMessage<TopApplication>(this, msg.SelectedApplication));
            }
        }

        private void GetAssociatedApplications()
        {
            // Only get associated applications if they haven't already been gathered (i.e. the selected tiles application collection is empty)
            // Update 17/07/15: Always gather applications incase application details have been edited
            //if (SelectedTile.Applications.Any() == false)
            //{
                // Get all the associations associated with the selected item
                Messenger.Default.Send(new Message.M_GetAssociatedApplications(SelectedTile));
            //}

            if (SelectedTile.Applications.Any())
            {
                if (SelectedTile.Applications.Count == 1)
                {
                    // Only one application is associated with the selected item. Pass the application details to the applications tab for display.
                    TopApplication application = SelectedTile.Applications[0] as TopApplication;
                    Messenger.Default.Send<GenericMessage<TopApplication>>(new GenericMessage<TopApplication>(application));
                }
                else
                {
                    DisplayTileTab();
                    DisplayApplicationsAsTiles();
                }
            }
            else
            {
                DisplayTileTab();
            }
        }

        private void RefreshTiles(Project project)
        {
            if (project != null)
            {
                if (project.SubItems.Any())
                {
                    // Reset the active tile collection and add all subitems of the selected tile
                    ActiveTileCollection = new ObservableCollection<SwitcherItem>(project.SubItems);
                }
            }
            else
            {
                ActiveTileCollection = new ObservableCollection<SwitcherItem>(ProjectsCollection);
            }
            
        }

        /// <summary>
        /// Changes the active project.
        /// </summary>
        public void ChangeActiveProject(Message.M_SimpleAction msg)
        {
            if (msg.SimpleAction == Message.M_SimpleAction.Action.ChangeActiveProject)
            {
                if (SelectedTile.IsActiveProject == false)
                {
                    SelectedTile.IsActiveProject = true;

                    // Unselect all other projects
                    foreach (Project project in ProjectsCollection.Where(proj => proj.Name != SelectedTile.Name))
                    {
                        project.IsActiveProject = false;
                        project.ChangeIsActiveForSubProjects(SelectedTile.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Displays the context menus. Triggered by a right-click on a tile.
        /// </summary>
        /// <param name="msg">Message containing the grid control contained within tile that has been clicked.</param>
        public void DisplayContextMenus(Grid grid)
        {
            Project project = grid.DataContext as Project;

            if (project != null) { grid.ContextMenu.ItemsSource = project.CombinedContextMenuCollection; }
            else
            {
                TopApplication application = grid.DataContext as TopApplication;

                if (application != null) { grid.ContextMenu.ItemsSource = application.ContextMenuCollection; }
            }
        }

        /// <summary>
        /// Changes the tile collection to the selected item from the title bar.
        /// </summary>
        /// <param name="msg">Message containing the selected item.</param>
        public void GoBackToParent(GenericMessage<SwitcherItem> msg)
        {
            if (msg.Sender is App | msg.Sender is MainWindow)
            {
                DisplayTileTab();

                Project switcherItem = msg.Content as Project;

                if (switcherItem == null)
                {
                    // Selected item has no parent. Reset active tile collection to all projects
                    ActiveTileCollection = new ObservableCollection<SwitcherItem>(ProjectsCollection);
                    SelectedTile = null;
                }
                else
                {
                    if (switcherItem.SubItems.Any())
                    {
                        // TODO Sure this can be done in the better way. Think you can link a class to a control template so I wouldn't have to create a new tile object each time.
                        ActiveTileCollection = new ObservableCollection<SwitcherItem>(switcherItem.SubItems);

                        SelectedTile = switcherItem;

                        // Send a message to the main view to update the breadcrumb
                        Messenger.Default.Send(new Message.M_UpdateSelectedTile(SelectedTile, this));
                    }
                }
            }
        }

        private void SaveChangesToProject(Message.M_EditTile msg)
        {
            if (msg.Sender is VM_EditTile)
            {
                Project project = msg.SelectedTile;

                if (project.IsDeleted)
                {
                    DeleteProject(project);
                }
                else if (project.IsNew)
                {
                    AddNewProject(project);
                }

                RefreshTiles(SelectedTile);
                Messenger.Default.Send(new Message.M_SettingsHaveBeenChanged(true));
                Messenger.Default.Send(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayTilesTab));
            }
        }

        private void AddNewProject(Project project)
        {
            if (SelectedTile != null)
            {
                // There is a tile selected so add this new project to it's collection of sub projects
                SelectedTile.SubItems.Add(project);
            }
            else
            {
                // No selected tile so project must be top level
                ProjectsCollection.Add(project);
            }
        }

        private void DeleteProject(Project project)
        {
            if (SelectedTile != null)
            {
                // There is a tile selected so remove this new project from it's collection of sub projects
                SelectedTile.SubItems.Remove(project);
            }
            else
            {
                // No selected tile so project must be top level
                ProjectsCollection.Remove(project);
            }
        }

        private void ChangeView(Message.M_ChangeView msg)
        {
            if (msg.View == Message.M_ChangeView.ViewToSelect.DisplayTilesTab)
            {
                // Toggle off & then on again to trigger visibility animation
                IsTileTabSelected = false;
                IsTileTabSelected = true;
            }
            else
            {
                IsTileTabSelected = false;
            }
        }

        private void DisplayTileTab()
        {
            Messenger.Default.Send(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayTilesTab));
        }
    }
}