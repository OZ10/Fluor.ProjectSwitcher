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

                if (isSelected == true)
                {
                    IsVisible = true;
                }

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

        private Visibility visibility;
        [XmlIgnore]
        public Visibility Visibility { get { return visibility; }
            set
            {
                visibility = value;
                RaisePropertyChanged("Visibility");
            }
        }

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
                    IsSelected = false;
                }

                RaisePropertyChanged("IsVisible");
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

        public void Setup(string applicationName, string installPath, string exe, bool isSelected, bool isVisible, bool isEnabled, string parentApplication)
        {
            Name = applicationName;
            Exe = exe;
            IsSelected = isSelected;
            IsVisible = isVisible;
            ParentItem = parentApplication;
            IsEnabled = isEnabled;
            InstallPath = installPath;
        }
    }
}
