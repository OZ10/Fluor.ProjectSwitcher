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
        public bool HasBeenChanged { get; set; }

        [XmlAttribute]
        public double Version { get; set; }

        [XmlAttribute]
        public int UserVersion { get; set; }

        [XmlAttribute]
        public string ActiveTheme { get; set; }

        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<TopApplication> Applications { get; set; }

        public ProjectSwitcherSettings()
        {
            HasBeenChanged = false;
            Projects = new ObservableCollection<Project>();
            Applications = new ObservableCollection<TopApplication>();

            if (ActiveTheme == "") ActiveTheme = "/Themes/Accents_Default.xaml";
        }
    }
}
