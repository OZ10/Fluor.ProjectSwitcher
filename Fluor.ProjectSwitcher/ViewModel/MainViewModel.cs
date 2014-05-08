using GalaSoft.MvvmLight;
using Ini;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using GalaSoft.MvvmLight.Messaging;
using System.Xml.Linq;
using System.Linq;
using Fluor.ProjectSwitcher.Class;

namespace Fluor.ProjectSwitcher.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        IniFile _sppidIni;

        public ObservableCollection<ProjectBase> ProjectsCollection { get; set; }
        public ObservableCollection<Class.ApplicationBase> ApplicationsCollection { get; set; }
        public ObservableCollection<Class.ApplicationBase> AssociatedApplicationCollection { get; set; }

        ProjectBase selectedSPPIDProject;
        public ProjectBase SelectedSPPIDProject
        {
            get
            {
                return selectedSPPIDProject;
            }
            set
            {
                selectedSPPIDProject = value;
                RaisePropertyChanged("SelectedSPPIDProject");
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ProjectsCollection = new ObservableCollection<ProjectBase>();
            ApplicationsCollection = new ObservableCollection<Class.ApplicationBase>();
            AssociatedApplicationCollection = new ObservableCollection<ApplicationBase>();

            Messenger.Default.Register<Message.ChangeSelectedProjectMessage>(this, ChangeSelectedProject);
        }

        /// <summary>
        /// Setups the environment.
        /// </summary>
        public void SetupEnvironment()
        {
            //CHECK IF THE SMARTPLANTV4.INI EXISTS
            if (File.Exists(Environment.ExpandEnvironmentVariables("%userprofile%\\SmartPlantManager.INI")))
            {
                _sppidIni = new IniFile(Environment.ExpandEnvironmentVariables("%userprofile%\\SmartPlantManager.INI"));

                //GET SPENG & SPPID ALL INSTALLATION DIRECTORIES
                string spemInstallPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Intergraph\SmartPlant Engineering Manager\CurrentVersion", "PathName", null);
                string sppidInstallPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Intergraph\SmartPlant P&ID\CurrentVersion", "InstallationPath", null);

                if (spemInstallPath != "" || sppidInstallPath != "")
                {
                    if (File.Exists("SPPIDProjects.xml"))
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load("SPPIDProjects.xml");

                        XElement x = XElement.Load("Fluor.ProjectSwitcher.Projects.xml");

                        PopulateProjects(x);

                        PopulateApplications(x);
                        //PopulateApplications(spemInstallPath, sppidInstallPath, xmlDoc);

                        SelectActiveProject();
                    }
                    else
                    {
                        MessageBox.Show("The configuration file -- SPPIDProjects.xml -- is missing from the source directory.", "Missing XML File", MessageBoxButton.OK, MessageBoxImage.Stop);
                        //TODO this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("The installation path for either SmartPlant Engineering Manager and/or SmartPlant P&ID cannot be found", "Installation Path", MessageBoxButton.OK, MessageBoxImage.Stop);
                    //TODO this.Close();
                }
            }
            else
            {
                MessageBox.Show("SmartPlant Engineering Manager ini file not found in your profile.\n\nPlease open SmartPlant Engineering Manager, manually connect to a project " +
                    "and then re-run this application.", "Missing INI", MessageBoxButton.OK, MessageBoxImage.Stop);
                //TODO this.Close();
            }
        }


        /// <summary>
        /// Reads the project details from the xml file and populates the projects listview.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        public void PopulateProjects(XElement xmlDoc)
        {
            try
            {
                ProjectBase project;
                ProjectBase subProject;

                foreach (XElement xmlProjects in xmlDoc.Elements("PROJECTS"))
                {
                    foreach (XElement xmlProject in xmlProjects.Elements("PROJECT"))
                    {
                        project = new ProjectBase();
                        project.Name = xmlProject.Attribute("NAME").Value;
                        project.ID = xmlProject.Attribute("ID").Value;

                        project.Applications = xmlProject.Attribute("APPLICATIONS").Value;

                        foreach (XElement xmlSubProject in xmlProject.Elements("SUBPROJECT"))
                        {
                            subProject = new ProjectBase();
                            subProject.Name = xmlSubProject.Attribute("NAME").Value;
                            subProject.ID = xmlSubProject.Attribute("ID").Value;

                            //TODO Need to add option to add applications at subapplication level (should this be allowed?)
                            if (xmlSubProject.Attribute("APPLICATIONS") == null)
                            {
                                subProject.Applications = project.Applications;
                            }

                            project.SubProjects.Add(subProject);
                        }

                        ProjectsCollection.Add(project);
                    }
                }
                

                //XmlNodeList elemList = xmlDoc.GetElementsByTagName("PROJECT");
                //for (int i = 0; i < elemList.Count; i++)
                //{
                //    project = new SPPIDProject();
                //    project.Name = elemList[i].Attributes["NAME"].Value;
                //    project.PlantName = elemList[i].Attributes["PLANTNAME"].Value;
                //    project.IniFile = elemList[i].Attributes["INIFILE"].Value;
                //    project.PIDPath = elemList[i].Attributes["PIDPATH"].Value;
                //    project.SPENGPath = elemList[i].Attributes["SPENGPATH"].Value;
                //    ProjectsCollection.Add(project);
                //}

                Messenger.Default.Send(new Message.PopulateProjectsMessage(ProjectsCollection));
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("The SPPIDProjects section of the configuration XML file contains errors.\n\nMandatory attributes are:\nNAME\nPLANTNAME\nINIFILE\nPIDPATH\nSPENGPATH",
                    "XML Errors", MessageBoxButton.OK, MessageBoxImage.Stop);
                throw;
            }
        }

        public void PopulateApplications(XElement xmlDoc)
        {
            Class.ApplicationBase application;
            foreach (XElement xmlApplication in xmlDoc.Elements().Where(app => app.Name != "PROJECTS"))
            {
                application = new ApplicationBase();
                application.Name = xmlApplication.Attribute("NAME").Value;

                foreach (XElement xmlAssociatedProject in xmlApplication.Elements("ASSOCIATED_PROJECTS"))
                {
                    ProjectBase sppidProject;
                    foreach (XElement xmlProject in xmlAssociatedProject.Elements("PROJECT"))
                    {
                        sppidProject = new ProjectBase();
                        sppidProject.Name = xmlProject.Attribute("NAME").Value;
                        sppidProject.ID = xmlProject.Attribute("ID").Value;
                        sppidProject.Parameters = xmlProject.Attribute("PARAMETERS").Value;
                        //sppidProject.PlantName = xmlProject.Attribute("PLANTNAME").Value;
                        //sppidProject.IniFile = xmlProject.Attribute("INIFILE").Value;
                        //sppidProject.PIDPath = xmlProject.Attribute("PIDPATH").Value;
                        //sppidProject.SPENGPath = xmlProject.Attribute("SPENGPATH").Value;

                        application.AssociatedProjects.Add(sppidProject);
                    }
                }

                Class.SubApplication subApplication;
                Class.SubApplication subSubApplication;
                foreach (XElement xmlSubApplication in xmlApplication.Elements("SUBAPPLICATION"))
                {
                    subApplication = new SubApplication();
                    subApplication.Name = xmlSubApplication.Attribute("NAME").Value;
                    subApplication.InstallPath = xmlSubApplication.Attribute("INSTALLPATH").Value;
                    subApplication.IsExpanded = (bool)xmlSubApplication.Attribute("ISEXPANDED");
                    subApplication.Exe = xmlSubApplication.Attribute("EXE").Value;
                    subApplication.IsSelected = (bool)xmlSubApplication.Attribute("ISSELECTED");
                    subApplication.IsVisible = (bool)xmlSubApplication.Attribute("ISVISIBLE");

                    foreach (XElement xmlSubSubApplication in xmlSubApplication.Elements("SUBAPPLICATION"))
                    {
                        subSubApplication = new SubApplication();
                        subSubApplication.Name = xmlSubSubApplication.Attribute("NAME").Value;

                        //TODO Need to add option to add an install path at subsubapplication level (should this be allowed?)
                        if (xmlSubSubApplication.Attribute("INSTALLPATH").Value == "")
                        {
                            subSubApplication.InstallPath = subApplication.InstallPath;
                        }
                        subSubApplication.IsExpanded = (bool)xmlSubSubApplication.Attribute("ISEXPANDED");
                        subSubApplication.Exe = xmlSubSubApplication.Attribute("EXE").Value;
                        subSubApplication.IsSelected = (bool)xmlSubSubApplication.Attribute("ISSELECTED");
                        subSubApplication.IsVisible = (bool)xmlSubSubApplication.Attribute("ISVISIBLE");

                        subApplication.SubApplications.Add(subSubApplication);
                    }

                    application.SubApplications.Add(subApplication);
                }

                ApplicationsCollection.Add(application);
            }
        }

        ///// <summary>
        ///// Reads the application details from the xml file and populates the applications listview.
        ///// </summary>
        ///// <param name="spemInstallPath">The spem install path.</param>
        ///// <param name="sppidInstallPath">The sppid install path.</param>
        ///// <param name="xmlDoc">The XML document.</param>
        //public void PopulateApplications(string spemInstallPath, string sppidInstallPath, XmlDocument xmlDoc)
        //{
        //    try
        //    {
        //        SPPIDApp sa;
        //        XmlNodeList elemList = xmlDoc.GetElementsByTagName("APP");
        //        for (int i = 0; i < elemList.Count; i++)
        //        {
        //            sa = new SPPIDApp();
        //            sa.Name = elemList[i].Attributes["NAME"].Value;
        //            sa.ParentApp = elemList[i].Attributes["PARENTAPP"].Value;
        //            sa.Exe = elemList[i].Attributes["EXE"].Value;

        //            //SET THE INSTALLATION PATH BASED ON THE PARENT APPLICATION -- DRAWING MANAGER IS INSTALLED UNDER THE P&ID WORKSTATION DIRECTORY SO ITS PARENT IS "SPPID"
        //            if (sa.ParentApp == "SPENG")
        //            {
        //                sa.ExeFullPath = spemInstallPath + "\\Program\\" + sa.Exe;
        //            }
        //            else if (sa.ParentApp == "SPPID")
        //            {
        //                sa.ExeFullPath = sppidInstallPath + "\\Program\\" + sa.Exe;
        //            }

        //            //IF THE APPLICATION IS MARKED AS "ISSELECTED = TRUE" THEN SET THE APPLICATION CHECKBOX TO CHECKED
        //            if (elemList[i].Attributes["ISSELECTED"].Value == "TRUE")
        //            {
        //                sa.IsChecked = true;
        //            }

        //            if (elemList[i].Attributes["ISVISIBLE"].Value == "TRUE")
        //            {
        //                sa.IsVisible = Visibility.Visible;
        //            }
        //            else
        //            {
        //                sa.IsVisible = Visibility.Collapsed;
        //            }

        //            //ADD APPLICATION TO COLLECTION
        //            ApplicationCollection.Add(sa);
        //        }

        //        Messenger.Default.Send(new Message.PopulateApplicationsMessage(ApplicationCollection));
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("The SPPIDApps section of the configuration XML file contains errors.\n\nMandatory attributes are:\nNAME\nPARENTAPP\nEXE\nISELECTED",
        //            "XML Errors", MessageBoxButton.OK, MessageBoxImage.Stop);
        //        throw;
        //    }
        //}

        /// <summary>
        /// Reads the active (last connected) project from the SmartPlantv4.ini and selects it in the projects listview.
        /// </summary>
        private void SelectActiveProject()
        {
            //string sppidIniPath = _sppidIni.IniReadValue("SmartPlant Manager", "SiteServer");
            //string sppidPlantName = _sppidIni.IniReadValue("SmartPlant Manager", "ActivePlant");

            //foreach (SPPIDProject sppidProject in ProjectsCollection)
            //{
            //    if (sppidProject.IniFile == sppidIniPath && sppidProject.PlantName == sppidPlantName)
            //    {
            //        SelectedSPPIDProject = sppidProject;
                    
            //        //SELECT THE ACTIVE PROJECT IN THE PROJECTS LISTVIEW
            //        Messenger.Default.Send(new Message.ChangeSelectedProjectMessage(sppidProject));

            //        sppidProject.IsActiveProject = true;
            //    }
            //}
        }

        /// <summary>
        /// Closes any open applications and opens new ones.
        /// </summary>
        public async void CloseAndOpenApplications()
        {
            //SHOW STATUS GRID WHICH COVERS ALL CONTROLS
            Messenger.Default.Send(new Message.StatusUpdateMessage(Visibility.Visible));

            //ONLY CLOSE OPEN APPLICATIONS IF THE SELECTED PROJECT IS NOT THE ACTIVE PROJECT
            if (SelectedSPPIDProject.IsActiveProject != true)
            {
                Messenger.Default.Send(new Message.StatusUpdateMessage(Visibility.Visible, "closing applications"));
                bool closeResult = await CloseApplicationsAsync();
            }

            ChangeActiveProject();
            SelectedSPPIDProject.SetProjectSpecificSettings();

            Messenger.Default.Send(new Message.StatusUpdateMessage(Visibility.Visible, "opening applications"));

            //OPEN NEW APPLICATIONS
            System.Threading.Thread.Sleep(1000);
            bool openResult = await OpenApplicationsAsync();

            //HIDE THE STATUS GRID
            Messenger.Default.Send(new Message.StatusUpdateMessage(Visibility.Hidden));
        }

        private void ChangeActiveProject()
        {
            //if (SelectedSPPIDProject.IsActiveProject == false)
            //{
            //    SelectedSPPIDProject.IsActiveProject = true;
            //    foreach (SPPIDProject sppidProject in ProjectsCollection) //lstProjects.Items)
            //    {
            //        if (sppidProject.Name != SelectedSPPIDProject.Name)
            //        {
            //            sppidProject.IsActiveProject = false;
            //        }
            //    }

            //    //SELECTED PROJECT IS NOT THE CURRENT ACTIVE PROJECT SO CHANGE CONNECTION DETAILS IN THE SMARTPLANTV4.INI
            //    ChangeSmartPlantv4INI(SelectedSPPIDProject);
            //}
        }

        /// <summary>
        /// Set ini plant and plant name in the smartplantv4.ini.
        /// </summary>
        /// <param name="sppidProject">The sppid project.</param>
        private void ChangeSmartPlantv4INI(SPPIDProject sppidProject)
        {
            _sppidIni.IniWriteValue("SmartPlant Manager", "SiteServer", sppidProject.IniFile);
            _sppidIni.IniWriteValue("SmartPlant Manager", "ActivePlant", sppidProject.PlantName);
            _sppidIni.IniWriteValue("SmartPlant Manager", "ActiveParentPlant", sppidProject.PlantName);
        }

        /// <summary>
        /// Closes the applications asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<bool> CloseApplicationsAsync()
        {
            return Task.Run<bool>(() => ClosingApplications());
        }

        /// <summary>
        /// Closes the applications.
        /// </summary>
        /// <returns></returns>
        private bool ClosingApplications()
        {
            // CHECK IF AN APPLICATION IS ALREADY OPEN
            foreach (ApplicationBase application in AssociatedApplicationCollection)
            {
                foreach (SubApplication subApplication in application.SubApplications)
                {
                    Process[] procs = Process.GetProcessesByName(subApplication.Exe.Replace(".exe", ""));

                    if (procs.Length > 0)
                    {
                        for (var i = 0; i < procs.Length; i++)
                        {
                            procs[i].CloseMainWindow();
                        }
                    }
                }
                
            }
            return true;
        }

        /// <summary>
        /// Opens the applications asynchronous.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        private Task<bool> OpenApplicationsAsync()
        {
            return Task.Run<bool>(() => OpeningApplications());
        }

        /// <summary>
        /// Opens the applications.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        private bool OpeningApplications()
        {
            try
            {
                //Process p;
                int i = 0;
                foreach (ApplicationBase application in AssociatedApplicationCollection)
                {
                    foreach (SubApplication subApplication in application.SubApplications.Where(subapp => subapp.IsSelected == true))
                    {
                        //if (sppidApp.IsChecked)
                        //if (subApplication.IsSelected)
                        //{
                        LaunchApplication(ref i, subApplication);
                        //}
                    }
                    
                }
                System.Threading.Thread.Sleep(2000 * i);
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Executable is missing from install directory.\n\nCheck the executable paths in the configuration XML file.", "Executable Missing", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
                //throw;
            }
        }

        private void LaunchApplication(ref int i, SubApplication subApplication)
        {
            Process p;

            //CHECK IF APPLICATION IS ALREADY OPEN
            Process[] processName = Process.GetProcessesByName(subApplication.Exe.Replace(".exe", ""));

            if (processName.Length == 0)
            {
                p = new Process();
                p.StartInfo.FileName = subApplication.InstallPath + subApplication.Exe; //sppidApp.ExeFullPath;
                p.Start();
                i += 1;
            }
        }

        private void ChangeSelectedProject(Message.ChangeSelectedProjectMessage changeSelectedProjectMessage)
        {
            AssociatedApplicationCollection = new ObservableCollection<ApplicationBase>();

            SelectedSPPIDProject = changeSelectedProjectMessage.SelectedProject;

            //Get application & sub applications of selected project
            //Get list of applications associated with the selected project
            foreach (string associatedApplication in SelectedSPPIDProject.ApplicationsList)
            {
                //var q = from projApp in SelectedSPPIDProject.Applications
                //        join apps in ApplicationsCollection on projApp equals apps.Name
                //        where apps.
                foreach (ApplicationBase application in ApplicationsCollection)
                {
                    // Set the active project

                    if (application.Name == associatedApplication)
                    {
                        foreach (ProjectBase project in application.AssociatedProjects)
                        {
                            if (project.ID == SelectedSPPIDProject.ID)
                            {
                                SelectedSPPIDProject = project;
                            }
                        }

                        foreach (SubApplication subApplication in application.SubApplications)
                        {
                            AssociatedApplicationCollection.Add(subApplication);
                        }
                    }
                }
            }

            Messenger.Default.Send(new Message.PopulateApplicationsMessage(AssociatedApplicationCollection));
        }
    }
}