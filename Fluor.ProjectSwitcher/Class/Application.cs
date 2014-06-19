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
using System.Collections.ObjectModel;

namespace Fluor.ProjectSwitcher.Class
{
    public class Application : Class.ProjectSwitcherItem 
    {
        //public string Name { get; set; }
        //public string ContextMenus { get; set; }
        //public List<SubApplication> SubApplications { get; set; }

        public Application(string applicationName, string contextMenus)
        {
            Name = applicationName;
            ContextMenus = contextMenus;

            SubItems = new ObservableCollection<ProjectSwitcherItem>();
        }

        public void GetSubApplications(XElement xmlApplication, SubApplication parentApplication) // string parentInstallPath, string parentContextMenu)
        {
            if (xmlApplication.Elements("SUBAPPLICATION").Any())
            {
                //Application has children. Set font to uppercase.
                Name = Name.ToUpper();

                SubApplication subApplication;
                foreach (XElement xmlSubApplication in xmlApplication.Elements("SUBAPPLICATION"))
                {

                    string installPath;
                    // Set the sub application's install path if it has been set
                    if (xmlSubApplication.Attribute("INSTALLPATH").Value != "")
                    {
                        installPath = xmlSubApplication.Attribute("INSTALLPATH").Value;
                    }
                    else
                    {
                        // Set the install path to the sub application's parent
                        if (parentApplication != null)
                        {
                            installPath = parentApplication.InstallPath;
                        }
                        else
                        {
                            installPath = "";
                        }
                    }

                    string contextMenu;
                    // Set the sub application's context menu parameters if it has been set
                    if (xmlSubApplication.Attribute("CONTEXTMENU").Value != "")
                    {
                        contextMenu = xmlSubApplication.Attribute("CONTEXTMENU").Value;
                    }
                    else
                    {
                        // Set the context menu parameters to those of the sub application's parent
                        if (parentApplication != null)
                        {
                            contextMenu = parentApplication.ContextMenus;
                        }
                        else
                        {
                            contextMenu = "";
                        }
                        
                    }

                    subApplication = new SubApplication(xmlSubApplication.Attribute("NAME").Value,
                                                        installPath,
                                                        (bool)xmlSubApplication.Attribute("ISEXPANDED"),
                                                        xmlSubApplication.Attribute("EXE").Value,
                                                        (bool)xmlSubApplication.Attribute("ISSELECTED"),
                                                        (bool)xmlSubApplication.Attribute("ISVISIBLE"),
                                                        contextMenu);

                    // Make the parent application expanded if the sub application is selected by default 
                    if (parentApplication != null && subApplication.IsSelected == true)
                    {
                        parentApplication.IsExpanded = true;
                        //parentApplication.IsSelected = null;
                    }

                    SubItems.Add(subApplication);

                    subApplication.GetSubApplications(xmlSubApplication, subApplication); // installPath, contextMenu);
                }
            }
        }
    }
}
