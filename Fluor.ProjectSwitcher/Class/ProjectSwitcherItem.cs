using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Fluor.ProjectSwitcher.Class
{
    public class ProjectSwitcherItem : ViewModelBase
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public bool IsExpanded { get; set; }
        public ObservableCollection<ProjectSwitcherItem> SubItems { get; set; }
        public string ContextMenus { get; set; }
        public string MiscText { get; set; }

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

        public ProjectSwitcherItem GetActiveSubItem()
        {
            ProjectSwitcherItem item = RecurseSubItems(SubItems);
            return item;
        }

        private ProjectSwitcherItem RecurseSubItems(ObservableCollection<ProjectSwitcherItem> items)
        {
            foreach (ProjectSwitcherItem subItem in items)
            {
                if (subItem.IsActive)
                {
                    return subItem;
                }

                RecurseSubItems(subItem.SubItems);
            }

            return null;
        }

        //public ProjectSwitcherItem GetSubItemUsingName(string itemName)
        //{
        //    ProjectSwitcherItem item = RecurseSubItems(SubItems, itemName);
        //    return item;
        //}

        //private ProjectSwitcherItem RecurseSubItems(ObservableCollection<ProjectSwitcherItem> items, string itemName)
        //{
        //    foreach (ProjectSwitcherItem subItem in SubItems)
        //    {
        //        if (itemName == subItem.Name)
        //        {
        //            return subItem;
        //        }

        //        RecurseSubItems(subItem.SubItems, itemName);
        //    }

        //    return null;
        //}

        public string GetContextMenuParameters()
        {
            ProjectSwitcherItem subItem = GetActiveSubItem();

            if (subItem != null)
            {
                return subItem.ContextMenus;
            }
            else
            {
                return "";
            }
        }

        public void CreateContextMenus(string contextMenuParameters, ref ObservableCollection<MenuItem> contextMenus)
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
                    contextMenu.Icon = CreateIcon();

                    contextMenus.Add(contextMenu);
                    //RaisePropertyChanged("ContextMenus");
                }
            }
        }

        private static Path CreateIcon()
        {
            string folderIconPath = "F1 M 21,30.0001L 55.9999,30.0001L 55.9999,50L 21,50L 21,30.0001 Z M 52,28L 37,28C 38,25 39.4999,24.0001 39.4999,24.0001L 50.75,24C 51.3023,24 52,24.6977 52,25.25L 52,28 Z ";

            Path path = new Path();
            path.Stretch = System.Windows.Media.Stretch.Uniform;
            path.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(00, 68, 125));
            path.StrokeThickness = 2;
            path.Height = 15;
            path.Width = 15;
            path.Data = Geometry.Parse(folderIconPath);
            return path;
        }
    }
}
