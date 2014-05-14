using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Fluor.ProjectSwitcher.Class
{
    public class ProjectSwitcherItem : ViewModelBase
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public bool IsExpanded { get; set; }
        public string ContextMenus { get; set; }

        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        public void CreateContextMenus(string contextMenuParameters, ref  ObservableCollection<MenuItem> contextMenus)
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
                if (contextMenus.Count > 0)
                {
                    //MenuItem contextMenu = new MenuItem();
                    //contextMenu.IsEnabled = false;
                    //contextMenu.Header = "SEP";
                    //ContextMenus.Add(contextMenu);
                    contextMenus.Add(null);
                }

                // Determine type
                if (parameter.StartsWith("(PATH)"))
                {
                    CreatePathContextMenu(parameter, contextMenus);
                }
                // TODO Different types - toggle applications settings? Set additional registry keys?
            }
        }

        private void CreatePathContextMenu(string parameter, ObservableCollection<MenuItem> contextMenus)
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

                    contextMenus.Add(contextMenu);
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
    }
}
