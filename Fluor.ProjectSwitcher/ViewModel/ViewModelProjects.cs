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
    public class ViewModelProjects : ViewModelBase 
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

        private ObservableCollection<Tile> topLevelProjectTileCollection;
        public ObservableCollection<Tile> TopLevelProjectTileCollection
        {
            get
            {
                return topLevelProjectTileCollection;
            }
            set
            {
                topLevelProjectTileCollection = value;
                RaisePropertyChanged("TopLevelProjectTileCollection");
            }
        }

        private ObservableCollection<Tile> activeProjectTileCollection;
        public ObservableCollection<Tile> ActiveProjectTileCollection
        {
            get
            {
                return activeProjectTileCollection;
            }
            set
            {
                activeProjectTileCollection = value;
                RaisePropertyChanged("ActiveProjectTileCollection");
            }
        }

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

        public ViewModelProjects()
        {
            Messenger.Default.Register<Message.MessagePopulateProjects>(this, UpdatedProjectsCollection);
            Messenger.Default.Register<NotificationMessage>(this, DisplayContextMenusMessage);
            Messenger.Default.Register<GenericMessage<Project>>(this, GoBackToParent);
        }

        private void UpdatedProjectsCollection(Message.MessagePopulateProjects populateProjectsMessage)
        {
            ProjectsCollection = populateProjectsMessage.ProjectsCollection;

            TopLevelProjectTileCollection = new ObservableCollection<Tile>();
            ActiveProjectTileCollection = new ObservableCollection<Tile>();

            foreach (Project project in ProjectsCollection)
            {
                Tile tile = CreateTile(project);

                TopLevelProjectTileCollection.Add(tile);
            }

            ActiveProjectTileCollection = TopLevelProjectTileCollection;
        }

        private Tile CreateTile(ProjectSwitcherItem project)
        {
            Tile tile = new Tile();
            tile.Click += new RoutedEventHandler(Project_Clicked);
            tile.DataContext = project;
            tile.Template = (ControlTemplate)System.Windows.Application.Current.Resources["TileTemplate"];

            return tile;
        }

        private void Project_Clicked(object sender, RoutedEventArgs e)
        {
            Tile tile = (Tile)sender;
            Project project = (Project)tile.DataContext;

            //SelectedProject = project;

            if (project.SubItems.Any())
            {
                ActiveProjectTileCollection = new ObservableCollection<Tile>();

                foreach (ProjectSwitcherItem subProject in project.SubItems)
                {
                    Tile tvi = CreateTile(subProject);

                    ActiveProjectTileCollection.Add(tvi);
                }
            }

            SelectedProject = project;
            Messenger.Default.Send<GenericMessage<Project>>(new GenericMessage<Project>(project));
        }

        //private void ChangedSelectedProject(GenericMessage<Project> selectedProjectMessage)
        //{
        //    Project selectedProject = (Project)selectedProjectMessage.Content;

        //    //TODO This needs to be refactored & recursive
        //    foreach (Tile tv in topLevelProjectTileCollection)
        //    {
        //        Project project = (Project)tv.DataContext;

        //        if (project.Name != selectedProject.Name)
        //        {
        //            project.IsActive = false;
        //        }

        //        foreach (ProjectSwitcherItem subProject in project.SubItems)
        //        {
        //            if (subProject.Name != selectedProject.Name)
        //            {
        //                subProject.IsActive = false;
        //            }
        //        }
        //    }
        //}

        // TODO This needs a better name!
        private void DisplayContextMenusMessage(NotificationMessage contextMenuMessage)
        {
            DisplayContextMenus(contextMenuMessage.Sender);
        }

        public void DisplayContextMenus(object sender)
        {
            // Triggered by a right-click on a project. The treeview does not change the selecteditem when right-clicking
            // so had to write this routine to change the selected item

            Tile tile = (Tile)sender;

            Project project = (Project)tile.DataContext; // GetSelectedProject(projectName);

            ContextMenus = new ObservableCollection<MenuItem>();

            // Send a message containing the project name to the main view model. The main view model returns the context
            // menu parameters as listed in the associations section
            Messenger.Default.Send(new NotificationMessageAction<string>(project, project.Name, (contextMenuParameters) =>
                {
                    project.CreateContextMenus(contextMenuParameters, ref contextMenus);
                }));

            tile.ContextMenu.ItemsSource = contextMenus;
        }

        //private Project GetSelectedProject(string projectName)
        //{
        //    // Loops through each project & subproject to find a project with the same name
        //    // as the project that has been right-clicked.
        //    // Sets the project to active is changes the treeview's selecteditem property
        //    foreach (Project project in projectsCollection)
        //    {
        //        if (project.Name == projectName)
        //        {
        //            project.IsActive = true;
        //            return project;
        //        }

        //        foreach (Project subProject in project.SubItems)
        //        {
        //            if (subProject.Name == projectName)
        //            {
        //                subProject.IsActive = true;
        //                return subProject;
        //            }

        //            if (subProject.SubItems.Any())
        //            {
        //                DisplayContextMenus(null);
        //            }
        //        }
        //    }

        //    return null;
        //}

        public void ProjectChanged(TreeView tv)
        {
            Project project = (Project)tv.SelectedItem;

            if (project != null)
            {
                // Project has changed. Send a message to the main view model to set the active project.
                Messenger.Default.Send(new Message.MessageChangeSelectedProject(project));
            }
        }

        public void GoBackToParent(GenericMessage<Project> message)
        {
            if (message.Content == null)
            {
                // Selected project had no parent. Reset to top level projects
                ActiveProjectTileCollection = TopLevelProjectTileCollection;
                SelectedProject = null;
            }
            else
            {
                // TODO Sure this can be done in the better way. Think you can link a class to a control template so I wouldn't have to create a new tile object each time.
                ActiveProjectTileCollection = new ObservableCollection<Tile>();

                Project project = (Project)message.Content;
                foreach (Project subProject in project.SubItems)
                {
                    Tile tile = CreateTile(subProject);
                    ActiveProjectTileCollection.Add(tile);
                }
                SelectedProject = project;
            }
            //Messenger.Default.Send<GenericMessage<Project>>(new GenericMessage<Project>(null));
        }
    }
}
