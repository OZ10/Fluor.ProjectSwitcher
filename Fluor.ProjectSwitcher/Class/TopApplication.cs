using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluor.ProjectSwitcher.Class;
using System.Xml.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Fluor.ProjectSwitcher.Class
{
    public class TopApplication : Class.SwitcherItem 
    {
        public TopApplication(string applicationName, IEnumerable<XElement> contextMenus, bool isEnabled)
        {
            Name = applicationName;
            MiscTextVisibility = Visibility.Collapsed;
            IsEnabled = isEnabled;

            GetContextMenus(contextMenus);

            //TileColor = (SolidColorBrush)System.Windows.Application.Current.Resources["WindowTitleColorBrush"];

            SubItems = new ObservableCollection<SwitcherItem>();
        }

        public void GetSubApplications(XElement xmlApplication, SubApplication parentApplication) // string parentInstallPath, string parentContextMenu)
        {
            if (xmlApplication.Elements("SUBAPPLICATION").Any())
            {
                SubApplication subApplication;
                foreach (XElement xmlSubApplication in xmlApplication.Elements("SUBAPPLICATION"))
                {
                    subApplication = new SubApplication(xmlSubApplication.Attribute("NAME").Value,
                                                        xmlSubApplication.Element("INSTALLPATH"),
                                                        xmlSubApplication.Attribute("EXE").Value,
                                                        (bool)xmlSubApplication.Attribute("ISSELECTED"),
                                                        (bool)xmlSubApplication.Attribute("ISVISIBLE"),
                                                        xmlSubApplication.Elements("CONTEXTMENUS").Elements("CONTEXTMENU"),
                                                        true,
                                                        parentApplication);

                    SubItems.Add(subApplication);

                    subApplication.GetSubApplications(xmlSubApplication, subApplication); // installPath, contextMenu);
                }
            }
        }
    }
}
