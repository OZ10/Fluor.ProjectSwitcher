using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Fluor.ProjectSwitcher.Class
{
    public class SubApplication : Class.Application
    {
        //public bool IsExpanded { get; set; }
        public string Exe { get; set; }

        private bool? isSelected;
        public bool? IsSelected
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

                if (installPath.StartsWith("HKEY") == true)
                {
                    // Array:
                    // [0] = Registry key location
                    // [1] = Key name
                    // [2] = String extentions to the path in the registry - i.e. registry value is "C:\programs\application" but applications exes are in "\bin\"
                    string[] reg = installPath.Split(',');
                    installPath = (string)Registry.GetValue(@reg[0], reg[1], null);

                    if (reg[2] != null)
                    {
                        installPath = installPath.Insert(installPath.Length, reg[2]);
                    }
                }
            }
        }

        public SubApplication(string applicationName, string installPath, bool isExpanded, string exe, bool isSelected, bool isVisible, string contextMenu) : base(applicationName, contextMenu)
        {
            Name = applicationName;
            InstallPath = installPath;
            IsExpanded = isExpanded;
            Exe = exe;
            IsSelected = isSelected;
            IsVisible = isVisible;
            ContextMenus = contextMenu;
        }
    }
}
