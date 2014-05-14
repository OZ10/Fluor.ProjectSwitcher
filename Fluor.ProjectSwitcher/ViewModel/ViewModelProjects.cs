using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Windows.Input;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class ViewModelProjects :ViewModelBase 
    {
        private ObservableCollection<Class.Project> projectsCollection;
        public ObservableCollection<Class.Project> ProjectsCollection
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

        private ObservableCollection<MenuItem> projectContextMenus;
        public ObservableCollection<MenuItem> ProjectContextMenus
        {
            get
            {
                return projectContextMenus;
            }
            set
            {
                projectContextMenus = value;
                RaisePropertyChanged("ProjectContextMenus");
            }
        }

        public ViewModelProjects()
        {
            Messenger.Default.Register<Message.MessagePopulateProjects>(this, UpdatedProjectsCollection);
        }

        private void UpdatedProjectsCollection(Message.MessagePopulateProjects populateProjectsMessage)
        {
            ProjectsCollection = populateProjectsMessage.ProjectsCollection;
        }

        public void DisplayContextMenus(string projectName)
        {
            // Triggered by a right-click on a project. The treeview does not change the selecteditem when right-clicking
            // so had to write  this routine to change the selected item

            Class.Project project = GetSelectedProject(projectName);

            ProjectContextMenus = new ObservableCollection<MenuItem>();

            // Send a message containing the project name to the main view model. The main view model returns the context
            // menu parameters as listed in the associations section
            Messenger.Default.Send(new NotificationMessageAction<string>(this, project.Name, (contextMenuParameters) =>
                {
                    Class.ContextMenus.CreateContextMenus(contextMenuParameters, ref projectContextMenus);
                }));
        }

        private Class.Project GetSelectedProject(string projectName)
        {
            // Loops through each project & subproject to find a project with the same name
            // as the project that has been right-clicked.
            // Sets the project to active is changes the treeview's selecteditem property
            foreach (Class.Project project in projectsCollection)
            {
                if (project.Name == projectName)
                {
                    project.IsActive = true;
                    return project;
                }

                foreach (Class.Project subProject in project.SubProjects)
                {
                    if (subProject.Name == projectName)
                    {
                        subProject.IsActive = true;
                        return subProject;
                    }

                    if (subProject.SubProjects.Any())
                    {
                        DisplayContextMenus(subProject.Name);
                    }
                }
            }

            return null;
        }

        public void ProjectChanged(TreeView tv)
        {
            Class.Project project = (Class.Project)tv.SelectedItem;

            if (project != null)
            {
                // Project has changed. Send a message to the main view model to set the active project.
                Messenger.Default.Send(new Message.MessageChangeSelectedProject(project));
            }
        }
    }
}
