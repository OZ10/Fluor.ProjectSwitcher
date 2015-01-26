using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Windows.Media;
using System.Windows;

namespace Fluor.ProjectSwitcher.Class
{
    public class Project : Class.SwitcherItem
    {
        public Project(string projectName, IEnumerable<XElement> contextMenus, string miscText, bool isEnabled, Project parentProject)
        {
            Name = projectName;
            MiscTextVisibility = Visibility.Collapsed;
            MiscText = miscText;
            IsEnabled = isEnabled;
            ParentItem = parentProject;
            IsEditMode = Visibility.Collapsed;

            GetContextMenus(contextMenus);

            //Array colorValues = Enum.GetValues(typeof(tileColors));
            //Random random = new Random();
            //tileColors s = (tileColors)colorValues.GetValue(random.Next(colorValues.Length));

            SubItems = new ObservableCollection<SwitcherItem>();
        }

        public void CreateSubProjects(XElement xmlProject) //, string parentContextMenu)
        {
            if (xmlProject.Elements("SUBPROJECT").Any())
            {
                // Application has children. Set font to uppercase.
                //Name = Name.ToUpper();

                Project subProject;

                foreach (XElement xmlSubProject in xmlProject.Elements("SUBPROJECT"))
                {
                    //TODO Does the subproject need to inherit it's parent's context menus??
                    // Set the context menu parameters to those of the sub project's parent
                    //string contextMenu = parentContextMenu;

                    subProject = new Project(xmlSubProject.Attribute("NAME").Value,
                                             xmlSubProject.Elements("CONTEXTMENUS").Elements("CONTEXTMENU"),
                                             xmlSubProject.Attribute("MISCTEXT").Value,
                                             true,
                                             this);

                    SubItems.Add(subProject);

                    subProject.CreateSubProjects(xmlSubProject); //, contextMenu);
                }
            } 
        }

        public void ChangeIsActiveForSubProjects(string selectedProjectName)
        {
            foreach (Project subProject in SubItems)
            {
                if (subProject.Name != selectedProjectName)
                {
                    subProject.IsActiveProject = false;
                }
                
                subProject.ChangeIsActiveForSubProjects(selectedProjectName);
            }
        }
    }
}
