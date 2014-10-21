using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Windows.Input;
using Fluor.ProjectSwitcher.Base.Class;
using System.Windows;
using MahApps.Metro.Controls;
using System.Windows.Media;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class ViewModelTiles : ViewModelBase 
    {
        // TODO Is this property required?
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

        // TODO Is this property required?
        private ObservableCollection<SwitcherItem> applicationsCollection;
        public ObservableCollection<SwitcherItem> ApplicationsCollection
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

        private SwitcherItem selectedTile;
        public SwitcherItem SelectedTile
        {
            get
            {
                return selectedTile;
            }
            set
            {
                selectedTile = value;
                RaisePropertyChanged("SelectedTile");

                // Send a message to the main view model containing the newly selected item
                Messenger.Default.Send<GenericMessage<SwitcherItem>>(new GenericMessage<SwitcherItem>(this, selectedTile));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelTiles"/> class.
        /// </summary>
        public ViewModelTiles()
        {
            Messenger.Default.Register<Message.MessagePopulateProjects>(this, UpdateProjectsCollection);
            Messenger.Default.Register<GenericMessage<Grid>>(this, DisplayContextMenus);
            Messenger.Default.Register<GenericMessage<SwitcherItem>>(this, GoBackToParent);
            Messenger.Default.Register<Message.MessagePopulateApplications>(this, UpdateApplicationsCollection);
        }

        /// <summary>
        /// Updates the projects collection and creates a tile for each project.
        /// </summary>
        /// <param name="msg">Message containing a collection of projects.</param>
        private void UpdateProjectsCollection(Message.MessagePopulateProjects msg)
        {
            ProjectsCollection = msg.ProjectsCollection;

            TopLevelTileCollection = new ObservableCollection<Button>();
            ActiveTileCollection = new ObservableCollection<Button>();

            foreach (Project project in ProjectsCollection)
            {
                Button tile = CreateTile(project);

                TopLevelTileCollection.Add(tile);
            }

            ActiveTileCollection = TopLevelTileCollection;
        }

        /// <summary>
        /// Updates the applications collection and creates a tile for each application.
        /// </summary>
        /// <param name="msg">Message containing a collection of applications.</param>
        private void UpdateApplicationsCollection(Message.MessagePopulateApplications msg)
        {
            ApplicationsCollection = msg.ApplicationsCollection;

            ActiveTileCollection = new ObservableCollection<Button>();

            foreach (TopApplication application in ApplicationsCollection)
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
            Button tile = new Button();
            tile.Click += new RoutedEventHandler(Tile_Clicked);
            tile.DataContext = switcherItem;
            tile.Style = (Style)System.Windows.Application.Current.Resources["MetroTileCustom"];
            
            return tile;
        }

        /// <summary>
        /// Handles the Clicked event of the Tile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Tile_Clicked(object sender, RoutedEventArgs e)
        {
            Button tile = (Button)sender;
            SwitcherItem switcherItem = (SwitcherItem)tile.DataContext;

            // Try to cast the item as a Project
            Project project = switcherItem as Project;

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

                // TODO Is this message required? switcherItem already has a collection of context menus.
                GetContextMenus(switcherItem);
                Messenger.Default.Send<GenericMessage<ObservableCollection<MenuItem>>>(new GenericMessage<ObservableCollection<MenuItem>>(switcherItem.ContextMenuCollection));
            }
            else
            {
                // Selected tile must be an application. Send a message to the main view model containing the selected application
                TopApplication application = switcherItem as TopApplication;
                Messenger.Default.Send<GenericMessage<TopApplication>>(new GenericMessage<TopApplication>(this, application));
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
                switcherItem.CreateContextMenus(contextMenuParameters);
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
                SwitcherItem switcherItem = (SwitcherItem)msg.Content;
                
                if (switcherItem == null)
                {
                    // Selected item had no parent. Reset to top level projects
                    ActiveTileCollection = TopLevelTileCollection;
                    SelectedTile = null;
                }
                else
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
                }
            }
            
        }
    }
}
