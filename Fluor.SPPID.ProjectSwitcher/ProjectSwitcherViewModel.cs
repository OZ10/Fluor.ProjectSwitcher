using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows;
using Ini;

namespace Fluor.SPPID.ProjectSwitcher
{
    public class ProjectSwitcherViewModel
    {
        public ObservableCollection<SPPIDProject> ProjectsCollection { get; set; }
        public ObservableCollection<SPPIDApp> ApplicationCollection { get; set; }

        public ProjectSwitcherViewModel()
        {
            ProjectsCollection = new ObservableCollection<SPPIDProject>();
            ApplicationCollection = new ObservableCollection<SPPIDApp>();
        }

        /// <summary>
        /// Reads the project details from the xml file and populates the projects listview.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        public void PopulateProjects(XmlDocument xmlDoc)
        {
            try
            {
                SPPIDProject sp;
                XmlNodeList elemList = xmlDoc.GetElementsByTagName("PROJECT");
                for (int i = 0; i < elemList.Count; i++)
                {
                    sp = new SPPIDProject();
                    sp.Name = elemList[i].Attributes["NAME"].Value;
                    sp.PlantName = elemList[i].Attributes["PLANTNAME"].Value;
                    sp.IniFile = elemList[i].Attributes["INIFILE"].Value;
                    sp.PIDPath = elemList[i].Attributes["PIDPATH"].Value;
                    sp.SPENGPath = elemList[i].Attributes["SPENGPATH"].Value;
                    ProjectsCollection.Add(sp);
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("The SPPIDProjects section of the configuration XML file contains errors.\n\nMandatory attributes are:\nNAME\nPLANTNAME\nINIFILE\nPIDPATH\nSPENGPATH",
                    "XML Errors", MessageBoxButton.OK, MessageBoxImage.Stop);
                throw;
            }
        }

        /// <summary>
        /// Reads the application details from the xml file and populates the applications listview.
        /// </summary>
        /// <param name="spemInstallPath">The spem install path.</param>
        /// <param name="sppidInstallPath">The sppid install path.</param>
        /// <param name="xmlDoc">The XML document.</param>
        public void PopulateApplications(string spemInstallPath, string sppidInstallPath, XmlDocument xmlDoc)
        {
            try
            {
                SPPIDApp sa;
                XmlNodeList elemList = xmlDoc.GetElementsByTagName("APP");
                for (int i = 0; i < elemList.Count; i++)
                {
                    sa = new SPPIDApp();
                    sa.Name = elemList[i].Attributes["NAME"].Value;
                    sa.ParentApp = elemList[i].Attributes["PARENTAPP"].Value;
                    sa.Exe = elemList[i].Attributes["EXE"].Value;

                    //SET THE INSTALLATION PATH BASED ON THE PARENT APPLICATION -- DRAWING MANAGER IS INSTALLED UNDER THE P&ID WORKSTATION DIRECTORY SO ITS PARENT IS "SPPID"
                    if (sa.ParentApp == "SPENG")
                    {
                        sa.ExeFullPath = spemInstallPath + "\\Program\\" + sa.Exe;
                    }
                    else if (sa.ParentApp == "SPPID")
                    {
                        sa.ExeFullPath = sppidInstallPath + "\\Program\\" + sa.Exe;
                    }

                    //IF THE APPLICATION IS MARKED AS "ISSELECTED = TRUE" THEN SET THE APPLICATION CHECKBOX TO CHECKED
                    if (elemList[i].Attributes["ISSELECTED"].Value == "TRUE")
                    {
                        sa.IsChecked = true;
                    }

                    if (elemList[i].Attributes["ISVISIBLE"].Value == "TRUE")
                    {
                        sa.IsVisible = Visibility.Visible;
                    }
                    else
                    {
                        sa.IsVisible = Visibility.Collapsed;
                    }

                    //ADD APPLICATION TO COLLECTION
                    ApplicationCollection.Add(sa);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The SPPIDApps section of the configuration XML file contains errors.\n\nMandatory attributes are:\nNAME\nPARENTAPP\nEXE\nISELECTED",
                    "XML Errors", MessageBoxButton.OK, MessageBoxImage.Stop);
                throw;
            }
        }
    }
}
