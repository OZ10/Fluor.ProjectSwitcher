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

namespace Fluor.SPPID.ProjectSwitcher.ViewModel
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

        public ObservableCollection<SPPIDProject> ProjectsCollection { get; set; }
        public ObservableCollection<SPPIDApp> ApplicationCollection { get; set; }

        SPPIDProject selectedSPPIDProject;
        public SPPIDProject SelectedSPPIDProject
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
            ProjectsCollection = new ObservableCollection<SPPIDProject>();
            ApplicationCollection = new ObservableCollection<SPPIDApp>();

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

                        PopulateProjects(xmlDoc);

                        PopulateApplications(spemInstallPath, sppidInstallPath, xmlDoc);

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

                Messenger.Default.Send(new Message.PopulateProjectsMessage(ProjectsCollection));
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

                Messenger.Default.Send(new Message.PopulateApplicationsMessage(ApplicationCollection));
            }
            catch (Exception)
            {
                MessageBox.Show("The SPPIDApps section of the configuration XML file contains errors.\n\nMandatory attributes are:\nNAME\nPARENTAPP\nEXE\nISELECTED",
                    "XML Errors", MessageBoxButton.OK, MessageBoxImage.Stop);
                throw;
            }
        }

        /// <summary>
        /// Reads the active (last connected) project from the SmartPlantv4.ini and selects it in the projects listview.
        /// </summary>
        private void SelectActiveProject()
        {
            string sppidIniPath = _sppidIni.IniReadValue("SmartPlant Manager", "SiteServer");
            string sppidPlantName = _sppidIni.IniReadValue("SmartPlant Manager", "ActivePlant");

            foreach (SPPIDProject sppidProject in ProjectsCollection)
            {
                if (sppidProject.IniFile == sppidIniPath && sppidProject.PlantName == sppidPlantName)
                {
                    SelectedSPPIDProject = sppidProject;
                    
                    //SELECT THE ACTIVE PROJECT IN THE PROJECTS LISTVIEW
                    Messenger.Default.Send(new Message.ChangeSelectedProjectMessage(sppidProject));

                    sppidProject.IsActiveProject = true;
                }
            }
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

            Messenger.Default.Send(new Message.StatusUpdateMessage(Visibility.Visible, "opening applications"));

            //OPEN NEW APPLICATIONS
            System.Threading.Thread.Sleep(1000);
            bool openResult = await OpenApplicationsAsync();

            //HIDE THE STATUS GRID
            Messenger.Default.Send(new Message.StatusUpdateMessage(Visibility.Hidden));
        }

        private void ChangeActiveProject()
        {
            if (SelectedSPPIDProject.IsActiveProject == false)
            {
                SelectedSPPIDProject.IsActiveProject = true;
                foreach (SPPIDProject sppidProject in ProjectsCollection) //lstProjects.Items)
                {
                    if (sppidProject.Name != SelectedSPPIDProject.Name)
                    {
                        sppidProject.IsActiveProject = false;
                    }
                }

                //SELECTED PROJECT IS NOT THE CURRENT ACTIVE PROJECT SO CHANGE CONNECTION DETAILS IN THE SMARTPLANTV4.INI
                ChangeSmartPlantv4INI(SelectedSPPIDProject);
            }
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
            //CHECK IF AN APPLICATION IS ALREADY OPEN
            foreach (SPPIDApp sppidApp in ApplicationCollection)
            {
                Process[] procs = Process.GetProcessesByName(sppidApp.Exe.Replace(".exe", ""));

                if (procs.Length > 0)
                {
                    for (var i = 0; i < procs.Length; i++)
                    {
                        procs[i].CloseMainWindow();
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
                Process p;
                int i = 0;
                foreach (SPPIDApp sppidApp in ApplicationCollection)
                {
                    if (sppidApp.IsChecked)
                    {
                        //CHECK IF APPLICATION IS ALREADY OPEN
                        Process[] processName = Process.GetProcessesByName(sppidApp.Exe.Replace(".exe", ""));

                        if (processName.Length == 0)
                        {
                            p = new Process();
                            p.StartInfo.FileName = sppidApp.ExeFullPath;
                            p.Start();
                            i += 1;
                        }
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

        private void ChangeSelectedProject(Message.ChangeSelectedProjectMessage changeSelectedProjectMessage)
        {
            SelectedSPPIDProject = changeSelectedProjectMessage.SelectedProject;
        }
    }
}