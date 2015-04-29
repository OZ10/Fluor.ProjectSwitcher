using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Fluor.ProjectSwitcher.Class
{
    public class SubApplication : Class.TopApplication
    {
        [XmlAttribute]
        public string Exe { get; set; }

        private bool isSelected;
        [XmlAttribute]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;

                if (SubItems != null)
                {
                    foreach (SubApplication subApplication in SubItems)
                    {
                        subApplication.IsSelected = value;
                    }

                    RaisePropertyChanged("IsSelected");
                }
                
            }
        }

        [XmlIgnore]
        public Visibility Visibility { get; set; }

        private bool isVisible;
        [XmlAttribute]
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;

                if (isVisible == true)
                {
                    Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private string installPath;
        [XmlAttribute]
        public string InstallPath
        {
            get
            {
                return installPath;
            }
            set
            {
                installPath = value;
            }
        }

        public SubApplication()
        {
        }

        public void Setup(string applicationName, XElement installPath, string exe, bool isSelected, bool isVisible, IEnumerable<XElement> contextMenu, bool isEnabled, Fluor.ProjectSwitcher.Class.SubApplication parentApplication)
        {
            Name = applicationName;
            Exe = exe;
            IsSelected = isSelected;
            IsVisible = isVisible;
            ParentItem = parentApplication;
            IsEnabled = isEnabled;

            if (installPath.Attribute("TYPE").Value != "")
            {
                Parameter parameter = new Parameter();
                parameter.Setup((Parameter.ParameterTypeEnum)Enum.Parse(typeof(Parameter.ParameterTypeEnum), installPath.Attribute("TYPE").Value), installPath.Attribute("VALUE").Value, installPath.Attribute("PATH").Value);
                InstallPath = parameter.GetInstallationPath();
            }
            else
            {
                InstallPath = parentApplication.InstallPath;
            }

            if (ParentItem != null && ParentItem.ContextMenuCollection.Any())
            {
                foreach (ContextMenu cm in ParentItem.ContextMenuCollection)
                {
                    ContextMenuCollection.Add(cm);
                }
            }

            GetContextMenus(contextMenu);
        }
    }
}
