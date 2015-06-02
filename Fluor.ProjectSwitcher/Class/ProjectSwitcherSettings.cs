using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Fluor.ProjectSwitcher.Class
{
    public class ProjectSwitcherSettings
    {
        [XmlAttribute]
        public bool HasBeenUpdated { get; set; }

        [XmlAttribute]
        public double Version { get; set; }

        [XmlAttribute]
        public int UserVersion { get; set; }

        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<TopApplication> Applications { get; set; }

        public ProjectSwitcherSettings()
        {
            HasBeenUpdated = false;
            Projects = new ObservableCollection<Project>();
            Applications = new ObservableCollection<TopApplication>();
        }
    }
}
