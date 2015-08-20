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
        [XmlIgnore]
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

        private ObservableCollection<ContextMenu> combinedContextMenuCollection;
        [XmlIgnore]
        public ObservableCollection<ContextMenu> CombinedContextMenuCollection
        {
            get
            {
                return combinedContextMenuCollection;
            }
            set
            {
                combinedContextMenuCollection = value;
                RaisePropertyChanged("CombinedContextMenuCollection");
            }
        }

        public ObservableCollection<Project> SubItems { get; set; }

        public Project()
        {
        }

        public void Setup(string projectName, ObservableCollection<ContextMenu> contextMenus, string miscText, bool isEnabled, string parentProject, bool isNew)
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
            CombinedContextMenuCollection = new ObservableCollection<ContextMenu>();
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

        public Project FindSubProjectByName(Project parentProject, string name)
        {
            if (parentProject.Name == name)
            {
                return parentProject;
            }
            else
            {
                foreach (var subProject in parentProject.SubItems)
                {
                    if (subProject.Name == name)
                    {
                        return subProject;
                    }

                    if (subProject.SubItems.Any())
                    {
                        FindSubProjectByName(subProject, name);
                    }
                }
            }

            return null;
        }

        public void CombineContextMenus()
        {
            CombinedContextMenuCollection = ContextMenuCollection.CloneJson<ObservableCollection<ContextMenu>>();

            foreach (var association in Associations)
            {
                foreach (var cm in association.ContextMenuCollection)
                {
                    if (!CombinedContextMenuCollection.Contains(cm))
                    {
                        CombinedContextMenuCollection.Add(cm);
                    }
                }
            }
        }
    }
}
