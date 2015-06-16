using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using Fluor.ProjectSwitcher.Class;
using System.Windows;
using System.Xml.Linq;
using System;
using System.Xml.Serialization;
using System.IO;

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

        private ObservableCollection<Button> topLevelTileCollection;
        public ObservableCollection<Button> TopLevelTileCollection
        {
            get
            {
                return topLevelTileCollection;
            }
            set
            {
                topLevelTileCollection = value;
                RaisePropertyChanged("TopLevelProjectTileCollection");
            }
        }

        private ObservableCollection<Button> activeTileCollection;
        public ObservableCollection<Button> ActiveTileCollection
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

        private Visibility isEditMode;
        public Visibility IsEditMode
        {
            get
            {
                return isEditMode;
            }
            set
            {
                isEditMode = value;
                RaisePropertyChanged("IsEditMode");
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
            Messenger.Default.Register<GenericMessage<Grid>>(this, DisplayContextMenus);
            Messenger.Default.Register<GenericMessage<SwitcherItem>>(this, GoBackToParent);
            Messenger.Default.Register<ObservableCollection<Project>>(this, PopulateProjects);
            Messenger.Default.Register<Message.M_EditTile>(this, SaveChangesToProject);
            Messenger.Default.Register<Message.M_ChangeView>(this, ChangeView);
            Messenger.Default.Register<Message.M_UpdateSelectedTile>(this, TileClicked);
            Messenger.Default.Register<Message.M_SimpleAction>(this, ChangeActiveProject);

            Messenger.Default.Register<GenericMessage<bool>>(this, SetEditMode);

            IsEditMode = Visibility.Collapsed;
        }

        /// <summary>
        /// Reads the project details from the xml file and populates the projects listview.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        private void PopulateProjects(ObservableCollection<Project> msg)
        {
            try
            {
                ProjectsCollection = msg;

                CreateTiles();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Errooooooooorrrrrr");
                throw;
            }
        }

        /// <summary>
        /// reates a tile for each project.
        /// </summary>
        /// <param name="msg">Message containing a collection of projects.</param>
        private void CreateTiles()
        {
            TopLevelTileCollection = new ObservableCollection<Button>();
            ActiveTileCollection = new ObservableCollection<Button>();

            AddNewButtonHeight = SetAddNewButtonHeight();

            foreach (Project project in ProjectsCollection)
            {
                Button tile = CreateTile(project);

                TopLevelTileCollection.Add(tile);
            }

            ActiveTileCollection = TopLevelTileCollection;
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
            ActiveTileCollection = new ObservableCollection<Button>();

            foreach (TopApplication application in SelectedTile.Applications) //  ApplicationsCollection)
            {
                Button tile = CreateTile(application);

                ActiveTileCollection.Add(tile);
            }
        }

        /// <summary>
        /// Creates a tile.
        /// </summary>
        /// <param name="switcherItem">The switcher item (project/application).</param>
        /// <returns></returns>
        private Button CreateTile(SwitcherItem switcherItem)
        {
            //TODO See if style can be set in XMAL
            Button tile = new Button();
            //tile.Click += new RoutedEventHandler(Tile_Clicked);
            tile.DataContext = switcherItem;
            tile.Style = (Style)System.Windows.Application.Current.Resources["MetroTileCustom"];

            return tile;
        }

        private void TileClicked(Message.M_UpdateSelectedTile msg)
        {
            if (msg.Sender != this)
            {
                // Try to cast the item as a Project
                Project project = msg.SelectedProject as Project;

                if (project != null)
                {
                    if (project.SubItems.Any())
                    {
                        // Reset the active tile collection and add all subitems of the selected tile
                        ActiveTileCollection = new ObservableCollection<Button>();

                        foreach (SwitcherItem subProject in project.SubItems)
                        {
                            Button tvi = CreateTile(subProject);

                            ActiveTileCollection.Add(tvi);
                        }
                    }

                    SelectedTile = project;

                    //ChangeActiveProject();

                    GetAssociatedApplications();
                }
                else
                {
                    // Selected tile must be an application. Send a message to the main view model containing the selected application
                    Messenger.Default.Send<GenericMessage<TopApplication>>(new GenericMessage<TopApplication>(this, msg.SelectedApplication));
                }
            }
            
        }

        private void GetAssociatedApplications()
        {
            // Only get associated applications if they haven't already been gathered (i.e. the selected tiles application collection is empty)
            if (SelectedTile.Applications.Any() == false)
            {
                // Get all the associations associated with the selected item
                Messenger.Default.Send(new Message.M_GetAssociatedApplications(SelectedTile));
            }

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
            //TODO Refactor
            if (project != null)
            {
                if (project.SubItems.Any())
                {
                    // Reset the active tile collection and add all subitems of the selected tile
                    ActiveTileCollection = new ObservableCollection<Button>();

                    foreach (SwitcherItem subProject in project.SubItems)
                    {
                        Button tvi = CreateTile(subProject);

                        ActiveTileCollection.Add(tvi);
                    }
                }
            }
            else
            {
                TopLevelTileCollection = new ObservableCollection<Button>();
                ActiveTileCollection = new ObservableCollection<Button>();

                foreach (Project proj in ProjectsCollection)
                {
                    Button tvi = CreateTile(proj);

                    TopLevelTileCollection.Add(tvi);
                    ActiveTileCollection.Add(tvi);
                }
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
        public void DisplayContextMenus(GenericMessage<Grid> msg)
        {
            Grid grid = (Grid)msg.Content;

            SwitcherItem switcherItem = (SwitcherItem)grid.DataContext;

            GetContextMenus(switcherItem);
            grid.ContextMenu.ItemsSource = switcherItem.ContextMenuCollection;
        }

        /// <summary>
        /// Get the context menus associated with the selected item.
        /// </summary>
        /// <param name="switcherItem">The switcher item.</param>
        private void GetContextMenus(SwitcherItem switcherItem)
        {
            // Send a message containing the switcher item's name to the main view model. The main view model returns the context
            // menu parameters as listed in the associations section
            Messenger.Default.Send(new NotificationMessageAction<string>(switcherItem, switcherItem.Name, (contextMenuParameters) =>
            {
                //switcherItem.CreateContextMenus(contextMenuParameters);
            }));
        }

        /// <summary>
        /// Changes the tile collection to the selected item from the title bar.
        /// </summary>
        /// <param name="msg">Message containing the selected item.</param>
        public void GoBackToParent(GenericMessage<SwitcherItem> msg)
        {
            if (msg.Sender is Fluor.ProjectSwitcher.App | msg.Sender is MainWindow)
            {
                DisplayTileTab();

                //SwitcherItem switcherItem = (SwitcherItem)msg.Content;
                Project switcherItem = msg.Content as Project;

                if (switcherItem == null)
                {
                    // Selected item had no parent. Reset to top level projects
                    ActiveTileCollection = TopLevelTileCollection;
                    SelectedTile = null;
                }
                else
                {
                    if (switcherItem.SubItems.Any())
                    {
                        // TODO Sure this can be done in the better way. Think you can link a class to a control template so I wouldn't have to create a new tile object each time.
                        ActiveTileCollection = new ObservableCollection<Button>();

                        // Create a tile for each of the item's subitems
                        foreach (SwitcherItem subProject in switcherItem.SubItems)
                        {
                            Button tile = CreateTile(subProject);
                            ActiveTileCollection.Add(tile);
                        }

                        SelectedTile = switcherItem;

                        // Send a message to the main view to update the breadcrumb
                        Messenger.Default.Send(new Message.M_UpdateSelectedTile(SelectedTile, this));
                    }
                }
            }
        }

        private void SetEditMode(GenericMessage<bool> msg)
        {
            //TODO This needs to be refactored 
            if (IsEditMode == Visibility.Collapsed)
            {
                IsEditMode = Visibility.Visible;

                foreach (Button tile in ActiveTileCollection)
                {
                    SwitcherItem swi = (SwitcherItem)tile.DataContext;
                    swi.IsEditMode = IsEditMode;
                }
            }
            else
            {
                IsEditMode = Visibility.Collapsed;

                foreach (Button tile in ActiveTileCollection)
                {
                    SwitcherItem swi = (SwitcherItem)tile.DataContext;
                    swi.IsEditMode = IsEditMode;
                }
            }
        }

        private void SaveChangesToProject(Message.M_EditTile msg)
        {
            //TODO Refactor
            if (msg.Sender is VM_Edit)
            {
                Project project = msg.SelectedTile;

                if (project.IsDeleted)
                {
                    if (SelectedTile != null)
                    {
                        // There is a tile selected so remove this new project to it's collection of sub projects
                        SelectedTile.SubItems.Remove(project);
                    }
                    else
                    {
                        // No selected tile so project must be top level
                        ProjectsCollection.Remove(project);
                    }
                }
                else if (project.IsNew)
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

                    //UpdateTiles();
                }
                RefreshTiles(SelectedTile);
                Messenger.Default.Send(new Message.M_SettingsHaveBeenChanged(true));
                Messenger.Default.Send(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayTilesTab));
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