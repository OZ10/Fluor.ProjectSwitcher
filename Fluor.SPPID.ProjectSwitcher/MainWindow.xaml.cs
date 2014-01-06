using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Ini;
using System.Xml;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Fluor.SPPID.ProjectSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        ObservableCollection<SPPIDProject> _sppidProjectsCollection = new ObservableCollection<SPPIDProject>();
        ObservableCollection<SPPIDApp> _sppidApplicationCollection = new ObservableCollection<SPPIDApp>();
        IniFile _sppidIni;
        BackgroundWorker _bgw;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
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

                        lstProjects.DataContext = _sppidProjectsCollection;
                        lstProjects.ItemsSource = _sppidProjectsCollection;

                        lstApps.DataContext = _sppidApplicationCollection;
                        lstApps.ItemsSource = _sppidApplicationCollection;

                        //foreach (ListViewItem lvi in lstApps.Items)
                        //{
                        //    lvi.Focusable = false;
                        //}


                        SelectActiveProject();
                    }
                    else
                    {
                        MessageBox.Show("The configuration file -- SPPIDProjects.xml -- is missing from the source directory.", "Missing XML File", MessageBoxButton.OK, MessageBoxImage.Stop);
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("The installation path for either SmartPlant Engineering Manager and/or SmartPlant P&ID cannot be found", "Installation Path", MessageBoxButton.OK, MessageBoxImage.Stop);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("SmartPlant Engineering Manager ini file not found in your profile.\n\nPlease open SmartPlant Engineering Manager, manually connect to a project " +
                    "and then re-run this application.", "Missing INI", MessageBoxButton.OK, MessageBoxImage.Stop);
                this.Close();
            }
        }

        /// <summary>
        /// Reads the project details from the xml file and populates the projects listview.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        private void PopulateProjects(XmlDocument xmlDoc)
        {
            try
            {
                SPPIDProject sp;
                XmlNodeList elemList = xmlDoc.GetElementsByTagName("PROJECT");
                for (int i = 0; i < elemList.Count; i++)
                {
                    sp = new SPPIDProject();
                    sp.ProjectName = elemList[i].Attributes["NAME"].Value;
                    sp.PlantName = elemList[i].Attributes["PLANTNAME"].Value;
                    sp.IniFile = elemList[i].Attributes["INIFILE"].Value;
                    sp.PIDPath = elemList[i].Attributes["PIDPATH"].Value;
                    sp.SPENGPath = elemList[i].Attributes["SPENGPATH"].Value;
                    _sppidProjectsCollection.Add(sp);
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
        private void PopulateApplications(string spemInstallPath, string sppidInstallPath, XmlDocument xmlDoc)
        {
            try
            {
                SPPIDApp sa;
                XmlNodeList elemList = xmlDoc.GetElementsByTagName("APP");
                for (int i = 0; i < elemList.Count; i++)
                {
                    sa = new SPPIDApp();
                    sa.AppName = elemList[i].Attributes["NAME"].Value;
                    sa.ParentApp = elemList[i].Attributes["PARENTAPP"].Value;
                    sa.Exe = elemList[i].Attributes["EXE"].Value;

                    //SET THE INSTALLATION PATH BASED ON THE PARENT APPLICATION -- DRAWING MANAGER IS INSTALLED UNDER THE P&ID WORKSTATION DIRECTORY SO ITS PARENT IS "SPPID"
                    if (sa.ParentApp == "SPENG")
                    {
                        sa.Exe = sa.Exe.Insert(0, spemInstallPath);
                    }
                    else if (sa.ParentApp == "SPPID")
                    {
                        sa.Exe = sa.Exe.Insert(0, sppidInstallPath);
                    }

                    //IF THE APPLICATION IS MARKED AS "ISSELECTED = TRUE" THEN SET THE APPLICATION CHECKBOX TO CHECKED
                    if (elemList[i].Attributes["ISSELECTED"].Value == "TRUE")
                    {
                        sa.IsChecked = true;
                    }

                    //ADD APPLICATION TO COLLECTION
                    _sppidApplicationCollection.Add(sa);
                }
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

            foreach (SPPIDProject sppidProject in _sppidProjectsCollection)
            {
                if (sppidProject.IniFile == sppidIniPath && sppidProject.PlantName == sppidPlantName)
                {
                    lstProjects.SelectedItem = sppidProject;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnOpenProject control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            CloseAndOpenApplications();
        }

        private async void CloseAndOpenApplications()
        {
            if (lstProjects.SelectedItem != null)
            {
                SPPIDProject sp = (SPPIDProject)lstProjects.SelectedItem;

                Process p = new Process();

                gdStatus.Visibility = System.Windows.Visibility.Visible;

                if (File.Exists("CheckRunning.exe"))
                {
                    //CLOSE OPEN APPLICATIONS
                    tbStatus.Text = "closing applications";
                    bool closeResult = await CloseApplicationsAsync(p);
                }
                else
                {
                    MessageBox.Show("CheckRunning.exe is missing from the source directory. Existing open SPENG and/or SPPID applications " +
                        "will not be closed.\n\nThe active project will be switched and new applications opened.", "Cannot Close Open Applications", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                //SET INI PATH AND PLANT NAME IN THE SMARTPLANTV4.INI
                _sppidIni.IniWriteValue("SmartPlant Manager", "SiteServer", sp.IniFile);
                _sppidIni.IniWriteValue("SmartPlant Manager", "ActivePlant", sp.PlantName);
                _sppidIni.IniWriteValue("SmartPlant Manager", "ActiveParentPlant", sp.PlantName);

                tbStatus.Text = "opening applications";
                bool openResult = await OpenApplicationsAsync(p);
                
                gdStatus.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private Task<bool> CloseApplicationsAsync(Process p)
        {
            return Task.Run<bool>(() => ClosingApplications(p));
        }

        private bool ClosingApplications(Process p)
        {
            p.StartInfo.FileName = "CheckRunning.exe";
            p.Start();
            p.WaitForExit(1000 * 60); //* 1);
            return true;
        }

        /// <summary>
        /// Opens each selected application.
        /// </summary>
        /// <param name="p">The process.</param>
        private Task<bool> OpenApplicationsAsync(Process p)
        {
            return Task.Run<bool>(() => OpeningApplications(p));
        }

        private bool OpeningApplications(Process p)
        {
            try
            {
                int i = 0;
                foreach (SPPIDApp sa in _sppidApplicationCollection)
                {
                    if (sa.IsChecked)
                    {
                        p = new Process();
                        p.StartInfo.FileName = sa.Exe;
                        p.Start();

                        i += 1;
                    }
                }
                System.Threading.Thread.Sleep(3000 * i);
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Executable is missing from install directory.\n\nCheck the executable paths in the configuration XML file.", "Executable Missing", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
                //throw;
            }
        }

        /// <summary>
        /// Handles the Click event of the MenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            if (mi != null)
            {
                switch (mi.Name)
                {
                    case "miSelectAll":
                        SelectApplications(true);
                        break;
                    case "miSelectNone":
                        SelectApplications(false);
                        break;
                    case "miSelectOnlyThis":
                        SelectApplications(false, mi);
                        break;
                    case "miSelectAllExceptThis":
                        SelectApplications(true, mi);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Selects / unselects the applications checkboxes.
        /// </summary>
        /// <param name="isChecked">if set to <c>true</c> [is checked].</param>
        /// <param name="mi">The mi.</param>
        private void SelectApplications(bool isChecked, MenuItem mi = null)
        {
            //GET THE SELECTED APPLICATION
            SPPIDApp selectedApp = null;
            if (lstApps.SelectedItem != null)
            {
                selectedApp = (SPPIDApp)lstApps.SelectedItem;
            }

            foreach (SPPIDApp sppidApp in _sppidApplicationCollection)
            {
                //IF APPLICATION IS NOT A SEPARATOR
                if (sppidApp.ParentApp != "SEP" && sppidApp.ParentApp != "HEADER")
                {
                    //IF A MENUITEM HAS BEEN PASSED IN THEN EITHER THE "SELECTONLYTHIS" OR "SELECTALLEXCEPTTHIS" BUTTON HAS BEEN CLICKED
                    if (mi != null)
                    {
                        if (selectedApp != null)
                        {
                            if (sppidApp.AppName == selectedApp.AppName)
                            {
                                //ASSIGN THE OPPOSITE ISCHECKED VALUE
                                sppidApp.IsChecked = !isChecked;
                            }
                            else
                            {
                                sppidApp.IsChecked = isChecked;
                            }
                        }
                    }
                    else
                    {
                        sppidApp.IsChecked = isChecked;
                    }
                }
                
            }
        }

        /// <summary>
        /// Handles the Click event of the Projects context menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            //GETS THE MENU ITEM THAT HAS BEEN CLICKED
            MenuItem mi = (MenuItem)sender;
            
            //GETS THE PROJECT THAT IS CURRENTLY SELECTED
            SPPIDProject sppidProject = (SPPIDProject)lstProjects.SelectedItem;

            if (sppidProject != null)
            {
                string argument = null;

                //OPEN WINDOWS EXPLORER
                Process p = new Process();
                p.StartInfo.FileName = "explorer.exe";

                //ADD A DIRECTORY PATH AS AN ARGUMENT
                switch (mi.Name)
                {
                    case "miOpenPIDReferenceData":
                        argument = sppidProject.PIDPath;
                        break;
                    case "miOpenEngineeringManagerPath":
                        argument = sppidProject.SPENGPath;
                        break;
                    default:
                        break;
                }
                p.StartInfo.Arguments = argument;
                p.Start();
            }
        }
    }
}
