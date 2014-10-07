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

        private ObservableCollection<MenuItem> contextMenus;
        public ObservableCollection<MenuItem> ContextMenus
        {
            get
            {
                return contextMenus;
            }
            set
            {
                contextMenus = value;
                RaisePropertyChanged("ContextMenus");
            }
        }

        private ObservableCollection<Tile> topLevelTileCollection;
        public ObservableCollection<Tile> TopLevelTileCollection
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

        private ObservableCollection<Tile> activeTileCollection;
        public ObservableCollection<Tile> ActiveTileCollection
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

        // TODO Is this property required?
        private Project selectedProject;
        public Project SelectedProject
        {
            get
            {
                return selectedProject;
            }
            set
            {
                selectedProject = value;
                RaisePropertyChanged("SelectedProject");
            }
        }

        public ViewModelTiles()
        {
            Messenger.Default.Register<Message.MessagePopulateProjects>(this, UpdatedProjectsCollection);
            Messenger.Default.Register<NotificationMessage>(this, DisplayContextMenus);
            Messenger.Default.Register<GenericMessage<Project>>(this, GoBackToParent);
            Messenger.Default.Register<Message.MessagePopulateApplications>(this, UpdateApplicationsCollection);
        }

        private void UpdatedProjectsCollection(Message.MessagePopulateProjects populateProjectsMessage)
        {
            ProjectsCollection = populateProjectsMessage.ProjectsCollection;

            TopLevelTileCollection = new ObservableCollection<Tile>();
            ActiveTileCollection = new ObservableCollection<Tile>();

            foreach (Project project in ProjectsCollection)
            {
                Tile tile = CreateTile(project);

                TopLevelTileCollection.Add(tile);
            }

            ActiveTileCollection = TopLevelTileCollection;
        }

        private void UpdateApplicationsCollection(Message.MessagePopulateApplications populateApplicationsMessage)
        {
            ApplicationsCollection = populateApplicationsMessage.ApplicationsCollection;

            ActiveTileCollection = new ObservableCollection<Tile>();

            foreach (Fluor.ProjectSwitcher.Base.Class.Application application in ApplicationsCollection)
            {
                Tile tile = CreateTile(application);

                ActiveTileCollection.Add(tile);
            }
        }

        private Tile CreateTile(ProjectSwitcherItem project)
        {
            Tile tile = new Tile();
            tile.Click += new RoutedEventHandler(Tile_Clicked);
            tile.DataContext = project;
            //tile.Template = (ControlTemplate)System.Windows.Application.Current.Resources["TileControlTemplate1"];
            tile.Template = (ControlTemplate)System.Windows.Application.Current.Resources["TileTemplate"];
            //tile.Style = (Style)System.Windows.Application.Current.Resources["MetroTileCustom"];

            return tile;
        }

        private void Tile_Clicked(object sender, RoutedEventArgs e)
        {
            Tile tile = (Tile)sender;
            ProjectSwitcherItem psItem = (ProjectSwitcherItem)tile.DataContext;

            Project project = psItem as Project;

            if (project != null)
            {
                if (project.SubItems.Any())
                {
                    ActiveTileCollection = new ObservableCollection<Tile>();

                    foreach (ProjectSwitcherItem subProject in project.SubItems)
                    {
                        Tile tvi = CreateTile(subProject);

                        ActiveTileCollection.Add(tvi);
                    }
                }

                SelectedProject = project;
                Messenger.Default.Send<GenericMessage<Project>>(new GenericMessage<Project>(this, project));
            }
            else
            {
                Fluor.ProjectSwitcher.Base.Class.Application application = psItem as Fluor.ProjectSwitcher.Base.Class.Application;
                Messenger.Default.Send<GenericMessage<Fluor.ProjectSwitcher.Base.Class.Application>>(new GenericMessage<Base.Class.Application>(this, application));
            }
        }

        public void DisplayContextMenus(NotificationMessage contextMenuMessage)
        {
            // Triggered by a right-click on a project. The treeview does not change the selecteditem when right-clicking
            // so had to write this routine to change the selected item

            Tile tile = (Tile)contextMenuMessage.Sender;

            ProjectSwitcherItem psItem = (ProjectSwitcherItem)tile.DataContext; // GetSelectedProject(projectName);

            ContextMenus = new ObservableCollection<MenuItem>();

            // Send a message containing the project name to the main view model. The main view model returns the context
            // menu parameters as listed in the associations section
            Messenger.Default.Send(new NotificationMessageAction<string>(psItem, psItem.Name, (contextMenuParameters) =>
                {
                    psItem.CreateContextMenus(contextMenuParameters, ref contextMenus);
                }));

            tile.ContextMenu.ItemsSource = contextMenus;
        }

        public void GoBackToParent(GenericMessage<Project> message)
        {
            if (message.Sender is MainViewModel)
            {
                if (message.Content == null)
                {
                    // Selected project had no parent. Reset to top level projects
                    ActiveTileCollection = TopLevelTileCollection;
                    SelectedProject = null;
                }
                else
                {
                    // TODO Sure this can be done in the better way. Think you can link a class to a control template so I wouldn't have to create a new tile object each time.
                    ActiveTileCollection = new ObservableCollection<Tile>();

                    Project project = (Project)message.Content;
                    foreach (Project subProject in project.SubItems)
                    {
                        Tile tile = CreateTile(subProject);
                        ActiveTileCollection.Add(tile);
                    }
                    SelectedProject = project;
                }
            }
            
        }
    }
}
