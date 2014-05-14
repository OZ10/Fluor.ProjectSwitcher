using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluor.ProjectSwitcher.Class;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using System.Windows.Media;
using System.Windows;

namespace Fluor.ProjectSwitcher.Class
{
    public class Application : Class.ProjectSwitcherItem 
    {
        //public string Name { get; set; }
        //public string ContextMenus { get; set; }
        public List<SubApplication> SubApplications { get; set; }

        public Application(string applicationName, string contextMenus)
        {
            Name = applicationName;
            ContextMenus = contextMenus;

            SubApplications = new List<SubApplication>();
        }

        public void GetSubApplications(XElement xmlApplication, string parentInstallPath, string parentContextMenu)
        {
            if (xmlApplication.Elements("SUBAPPLICATION").Any())
            {
                //Application has children. Set font to uppercase.
                Name = Name.ToUpper();

                SubApplication subApplication;
                foreach (XElement xmlSubApplication in xmlApplication.Elements("SUBAPPLICATION"))
                {
                    // Set the install path to that of the sub application's parent
                    string installPath = parentInstallPath;

                    // Set the sub application's install path if it has been set
                    if (xmlSubApplication.Attribute("INSTALLPATH").Value != "")
                    {
                        installPath = xmlSubApplication.Attribute("INSTALLPATH").Value;
                    }

                    // Set the context menu parameters to those of the sub application's parent
                    string contextMenu = parentContextMenu;

                    // Set the sub application's context menu parameters if it has been set
                    if (xmlSubApplication.Attribute("CONTEXTMENU").Value != "")
                    {
                        contextMenu = xmlSubApplication.Attribute("CONTEXTMENU").Value;
                    }

                    subApplication = new SubApplication(xmlSubApplication.Attribute("NAME").Value,
                                                        installPath,
                                                        (bool)xmlSubApplication.Attribute("ISEXPANDED"),
                                                        xmlSubApplication.Attribute("EXE").Value,
                                                        (bool)xmlSubApplication.Attribute("ISSELECTED"),
                                                        (bool)xmlSubApplication.Attribute("ISVISIBLE"),
                                                        contextMenu);

                    SubApplications.Add(subApplication);

                    subApplication.GetSubApplications(xmlSubApplication, installPath, contextMenu);
                }
            }

            
        }
    }
}
