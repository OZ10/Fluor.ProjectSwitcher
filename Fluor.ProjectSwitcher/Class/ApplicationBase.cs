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
    public class ApplicationBase : ViewModelBase 
    {
        public string Name { get; set; }
        public List<SubApplication> SubApplications { get; set; }

        public ApplicationBase(string applicationName)
        {
            Name = applicationName;

            SubApplications = new List<SubApplication>();
        }

        public void GetSubApplications(XElement xmlApplication, string parentInstallPath = "")
        {
            if (xmlApplication.Elements("SUBAPPLICATION").Any())
            {
                //Application has children. Set font to uppercase.
                Name = Name.ToUpper();

                SubApplication subApplication;
                foreach (XElement xmlSubApplication in xmlApplication.Elements("SUBAPPLICATION"))
                {
                    string installPath = xmlSubApplication.Attribute("INSTALLPATH").Value;

                    if (installPath == "")
                    {
                        installPath = parentInstallPath;
                    }

                    subApplication = new SubApplication(xmlSubApplication.Attribute("NAME").Value,
                                                        installPath,
                                                        (bool)xmlSubApplication.Attribute("ISEXPANDED"),
                                                        xmlSubApplication.Attribute("EXE").Value,
                                                        (bool)xmlSubApplication.Attribute("ISSELECTED"),
                                                        (bool)xmlSubApplication.Attribute("ISVISIBLE"));

                    SubApplications.Add(subApplication);

                    subApplication.GetSubApplications(xmlSubApplication, installPath);
                }
            }

            
        }
    }
}
