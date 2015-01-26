using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Fluor.ProjectSwitcher.Class
{
    public class SubApplication : Class.TopApplication
    {
        public string Exe { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;

                foreach (SubApplication subApplication in SubItems)
                {
                    subApplication.IsSelected = value;
                }

                RaisePropertyChanged("IsSelected");
            }
        }

        public Visibility Visibility { get; set; }

        private bool isVisible;
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

        public SubApplication(string applicationName, XElement installPath, string exe, bool isSelected, bool isVisible, IEnumerable<XElement> contextMenu, bool isEnabled, Fluor.ProjectSwitcher.Class.SubApplication parentApplication) : base(applicationName, contextMenu, isEnabled)
        {
            Name = applicationName;
            Exe = exe;
            IsSelected = isSelected;
            IsVisible = isVisible;
            ParentItem = parentApplication;

            if (installPath.Attribute("TYPE").Value != "")
            {
                Parameter parameter = new Parameter((Parameter.TypeEnum)Enum.Parse(typeof(Parameter.TypeEnum), installPath.Attribute("TYPE").Value), installPath.Attribute("VALUE").Value, installPath.Attribute("PATH").Value);
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
        }
    }
}
