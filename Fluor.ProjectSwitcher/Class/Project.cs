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
        private ObservableCollection<Association> _Associations;
        public ObservableCollection<Association> Associations
        {
            get
            {
                return _Associations;
            }
            set
            {
                _Associations = value;
                RaisePropertyChanged("Associations");
            }
        }

        private Association _selectedAssociation;
        public Association SelectedAssociation
        {
            get
            {
                return _selectedAssociation;
            }
            set
            {
                _selectedAssociation = value;
                RaisePropertyChanged("SelectedAssociation");
            }
        }

        public Project(string projectName, IEnumerable<XElement> contextMenus, string miscText, bool isEnabled, Project parentProject, bool isNew)
        {
            Name = projectName;
            OriginalName = projectName;
            MiscTextVisibility = Visibility.Collapsed;
            MiscText = miscText;
            IsEnabled = isEnabled;
            ParentItem = parentProject;
            IsEditMode = Visibility.Collapsed;
            IsNew = isNew;

            GetContextMenus(contextMenus);

            //Array colorValues = Enum.GetValues(typeof(tileColors));
            //Random random = new Random();
            //tileColors s = (tileColors)colorValues.GetValue(random.Next(colorValues.Length));

            SubItems = new ObservableCollection<SwitcherItem>();
            Associations = new ObservableCollection<Association>();
        }

        public void CreateSubProjects(XElement xmlProject, XElement xmlSettings) //, string parentContextMenu)
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
                                             this,
                                             false);

                    // Get any associations associated with this subproject
                    GetAssociations(subProject, xmlSettings);

                    SubItems.Add(subProject);

                    subProject.CreateSubProjects(xmlSubProject, xmlSettings); //, contextMenu);
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

        public void GetAssociations(Project project, XElement xmlSettings)
        {
            Association association;
            foreach (XElement xmlAssociation in xmlSettings.Elements("ASSOCIATIONS").Elements("ASSOCIATION"))
            {
                if (xmlAssociation.Attribute("PROJECTNAME").Value == project.Name)
                {
                    association = new Association(xmlAssociation.Attribute("PROJECTNAME").Value, xmlAssociation.Attribute("APPLICATIONNAME").Value,
                                                                    xmlAssociation.Elements("PARAMETERS").Elements("PARAMETER"), xmlAssociation.Elements("CONTEXTMENUS").Elements("CONTEXTMENU"));
                    project.Associations.Add(association);
                }
            }
        }
    }
}
