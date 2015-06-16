using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.Class
{
    public class Project : Class.SwitcherItem
    {
        private ObservableCollection<Association> _associations;
        public ObservableCollection<Association> Associations
        {
            get
            {
                return _associations;
            }
            set
            {
                _associations = value;
                RaisePropertyChanged("Associations");
            }
        }

        private Association _selectedAssociation;
        [XmlIgnore]
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

        private ObservableCollection<TopApplication> _applications;
        public ObservableCollection<TopApplication> Applications
        {
            get
            {
                return _applications;
            }
            set
            {
                _applications = value;
                RaisePropertyChanged("Applications");
            }
        }

        public ObservableCollection<Project> SubItems { get; set; }

        public Project()
        {
        }

        public void Setup(string projectName, ObservableCollection<ContextMenu> contextMenus, string miscText, bool isEnabled, Project parentProject, bool isNew)
        {
            Name = projectName;
            OriginalName = projectName;
            MiscTextVisibility = Visibility.Collapsed;
            MiscText = miscText;
            IsEnabled = isEnabled;
            ParentItem = parentProject;
            IsEditMode = Visibility.Collapsed;
            IsNew = isNew;

            ContextMenuCollection = contextMenus;

            //GetContextMenus(contextMenus);

            //Array colorValues = Enum.GetValues(typeof(tileColors));
            //Random random = new Random();
            //tileColors s = (tileColors)colorValues.GetValue(random.Next(colorValues.Length));

            SubItems = new ObservableCollection<Project>();
            Associations = new ObservableCollection<Association>();
            Applications = new ObservableCollection<TopApplication>();
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
                    //association = new Association(xmlAssociation.Attribute("PROJECTNAME").Value, xmlAssociation.Attribute("APPLICATIONNAME").Value,
                    //                                                xmlAssociation.Elements("PARAMETERS").Elements("PARAMETER"), xmlAssociation.Elements("CONTEXTMENUS").Elements("CONTEXTMENU"));
                    association = new Association();
                    //association.Setup(xmlAssociation.Attribute("PROJECTNAME").Value, xmlAssociation.Attribute("APPLICATIONNAME").Value,
                                                                    //xmlAssociation.Elements("PARAMETERS").Elements("PARAMETER"), xmlAssociation.Elements("CONTEXTMENUS").Elements("CONTEXTMENU"));
                    project.Associations.Add(association);
                }
            }
        }
    }
}
