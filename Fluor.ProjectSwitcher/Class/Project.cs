using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using System.Xml.Linq;
using System.Windows.Media;
using System.Windows;

namespace Fluor.ProjectSwitcher.Class
{
    public class Project : Class.ProjectSwitcherItem
    {
        public SolidColorBrush HighLightColor { get; set; }

        private bool _isActiveProject;
        public bool IsActiveProject
        {
            get
            {
                return _isActiveProject;
            }
            set
            {
                _isActiveProject = value;

                if (_isActiveProject == true)
                {
                    this.HighLightColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC27000");
                    RaisePropertyChanged("HighLightColor");
                }
                else
                {
                    this.HighLightColor = new SolidColorBrush(Colors.Black);
                    RaisePropertyChanged("HighLightColor");
                }
            }
        }

        public Project(string projectName, string projectID, bool isExpanded, string contextMenus, string miscText)
        {
            Name = projectName;
            ID = projectID;
            IsExpanded = isExpanded;
            ContextMenus = contextMenus;
            MiscText = miscText;
            this.HighLightColor = new SolidColorBrush(Colors.Black);

            SubItems = new ObservableCollection<ProjectSwitcherItem>();
        }

        public void CreateSubProjects(XElement xmlProject, string parentContextMenu)
        {
            if (xmlProject.Elements("SUBPROJECT").Any())
            {
                // Application has children. Set font to uppercase.
                Name = Name.ToUpper();

                Project subProject;

                foreach (XElement xmlSubProject in xmlProject.Elements("SUBPROJECT"))
                {
                    // Set the context menu parameters to those of the sub project's parent
                    string contextMenu = parentContextMenu;

                    // Set the sub project context menu parameters if it has been set
                    if (xmlSubProject.Attribute("CONTEXTMENU").Value != "")
                    {
                        contextMenu = xmlSubProject.Attribute("CONTEXTMENU").Value;
                    }

                    subProject = new Project(xmlSubProject.Attribute("NAME").Value,
                                             xmlSubProject.Attribute("ID").Value,
                                             (bool)xmlSubProject.Attribute("ISEXPANDED"),
                                             contextMenu,
                                             xmlSubProject.Attribute("MISCTEXT").Value);

                    SubItems.Add(subProject);

                    subProject.CreateSubProjects(xmlSubProject, contextMenu);
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
