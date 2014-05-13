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

            ContextMenus = new ObservableCollection<MenuItem>();

            // Send a message containing the project name to the main view model. The main view model returns the context
            // menu parameters as listed in the associations section
            Messenger.Default.Send(new NotificationMessageAction<string>(this, project.Name, (contextMenuParameters) =>
                {
                    CreateContextMenus(contextMenuParameters);
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

        private void CreateContextMenus(string contextMenuParameters)
        {
            // Associations have a 'contextmenu' field. This field contains context menu entries that can be
            // associated with either the project or application.
            // At present (v2.0) context menus can only be of type 'Path' - i.e. open window explorer to a location.

            // CONTEXTMENU Structure:
            // Menu types are seperated by ';'
            // Within each menu there is a type (Path etc), an association (either project or application) and one or more setting
            // The type + association and setting(s) are seperated by '#'

            // TYPE Structure:
            // Within the type is the type name in '( )' and a association

            // SETTING(S) Structure:
            // The setting(s) are seperated by ','

            // Method:
            // Split parameters from ContextMenu property
            // Determine what type of parameters there are
            // Carry out the command

            // Split out parameters
            string[] parameters = contextMenuParameters.Split(';');

            // For each parameter, determine type and set
            foreach (string parameter in parameters)
            {
                // If the context menus collection already contains items (added previously for a different application)
                // add a seperator.
                if (ContextMenus.Count > 1)
                {
                    //MenuItem contextMenu = new MenuItem();
                    //contextMenu.IsEnabled = false;
                    //contextMenu.Header = "SEP";
                    //ContextMenus.Add(contextMenu);
                    ContextMenus.Add(null);
                }
                
                // Determine type
                if (parameter.StartsWith("(PATH)"))
                {
                    CreatePathContextMenu(parameter);
                }
                // TODO Different types - toggle applications settings? Set additional registry keys?
            }
        }

        private void CreatePathContextMenu(string parameter)
        {
            // Split parameter into type & settings
            string[] pathTypeSettings = parameter.Split('#');

            // Array:
            // [0] = Path type & association
            // [X] = Path file settings
            // Remove type string to leave context menu location
            pathTypeSettings[0] = pathTypeSettings[0].Replace("(PATH)", "");

            // Check if association is 'Project', otherwise do nothing as context menu is not associated with the selected project
            if (pathTypeSettings[0] == "Project")
            {
                // Groups of context menus settings are seperated by '( )'.
                // Split context menu settings by ')(' - i.e. the finish ')' and start of a new group '('
                string[] pathSettings = pathTypeSettings[1].Split(new string[] { ")(" }, StringSplitOptions.None);

                // Split each group
                foreach (string pathSetting in pathSettings)
                {
                    // Array
                    // [0] = Context Menu display name
                    // [1] = Path
                    // Note the 'trims' to remove left over '(' or ')'
                    string[] setting = pathSetting.Split(',');

                    // Create a new context menu and add it to the ContextMenus collection
                    MenuItem contextMenu = new MenuItem();
                    contextMenu.Header = setting[0].TrimStart('(');
                    contextMenu.CommandParameter = setting[1].TrimEnd(')');

                    ContextMenus.Add(contextMenu);
                    //RaisePropertyChanged("ContextMenus");
                }
            }
        }

        public void OpenFolder(string folderPath)
        {
                // Open windows explorer
                Process p = new Process();
                p.StartInfo.FileName = "explorer.exe";
                p.StartInfo.Arguments = folderPath;
                p.Start();
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
