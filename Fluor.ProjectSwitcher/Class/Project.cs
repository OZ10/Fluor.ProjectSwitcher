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
        //public string Name { get; set; }
        //public string ID { get; set; }
        //public bool IsExpanded { get; set; }
        //public string ContextMenus { get; set; }
        public SolidColorBrush HighLightColor { get; set; }
        public ObservableCollection<Project> SubProjects { get; set; }

        //private bool isActive;
        //public bool IsActive
        //{
        //    get
        //    {
        //        return isActive;
        //    }
        //    set
        //    {
        //        isActive = value;
        //        RaisePropertyChanged("IsActive");
        //    }
        //}

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

        public Project(string projectName, string projectID, bool isExpanded, string contextMenus)
        {
            Name = projectName;
            ID = projectID;
            IsExpanded = isExpanded;
            ContextMenus = contextMenus;
            this.HighLightColor = new SolidColorBrush(Colors.Black);

            SubProjects = new ObservableCollection<Project>();
        }

        public void GetSubProjects(XElement xmlProject, string parentContextMenu)
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
                                             contextMenu);

                    SubProjects.Add(subProject);

                    subProject.GetSubProjects(xmlSubProject, contextMenu);
                }
            } 
        }

        public void ChangeIsActiveForSubProjects(string selectedProjectName)
        {
            foreach (Project subProject in SubProjects)
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
