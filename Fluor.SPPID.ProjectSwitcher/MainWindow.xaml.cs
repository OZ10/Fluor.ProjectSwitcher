using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Ini;
using System.Xml;
using Microsoft.Win32;

//TODO Change SPPIDApp class to make SPENG and/or Drawing Manager selected by default
//TODO Add right-click options to open path to P&ID Reference Data & PBS & Select All & Select None & Select Only This

namespace Fluor.SPPID.ProjectSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        ObservableCollection<SPPIDProject> _sppidProjectsCollection = new ObservableCollection<SPPIDProject>();
        ObservableCollection<SPPIDApp> _sppidAppCollection = new ObservableCollection<SPPIDApp>();

        public MainWindow()
        {
            InitializeComponent();

            //Get SPPID install directory
            string spemInstallPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Intergraph\SmartPlant Engineering Manager\CurrentVersion", "PathName", null);
            string sppidInstallPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Intergraph\SmartPlant P&ID\CurrentVersion", "InstallationPath", null);

            if (spemInstallPath != "" || sppidInstallPath != "")
            {
                SPPIDProject sp;
                SPPIDApp sa;

                var xmlDoc = new XmlDocument();
                xmlDoc.Load("SPPIDProject.xml");

                XmlNodeList elemList = xmlDoc.GetElementsByTagName("PROJECT");
                for (int i = 0; i < elemList.Count; i++)
                {
                    sp = new SPPIDProject();
                    sp.ProjectName = elemList[i].Attributes["NAME"].Value;
                    sp.PlantName = elemList[i].Attributes["PLANTNAME"].Value;
                    sp.IniFile = elemList[i].Attributes["INIFILE"].Value;
                    _sppidProjectsCollection.Add(sp);
                }

                elemList = xmlDoc.GetElementsByTagName("APP");
                for (int i = 0; i < elemList.Count; i++)
                {
                    sa = new SPPIDApp();
                    sa.AppName = elemList[i].Attributes["NAME"].Value;
                    sa.ParentApp = elemList[i].Attributes["PARENTAPP"].Value;
                    sa.Exe = elemList[i].Attributes["EXE"].Value;

                    if (sa.ParentApp == "SPENG")
                    {
                        sa.Exe = sa.Exe.Insert(0, spemInstallPath);
                    }
                    else if (sa.ParentApp == "SPPID")
                    {
                        sa.Exe = sa.Exe.Insert(0, sppidInstallPath);
                    }
                    
                    _sppidAppCollection.Add(sa);
                }

                lstProjects.DataContext = _sppidProjectsCollection;
                lstProjects.ItemsSource = _sppidProjectsCollection;

                lstApps.DataContext = _sppidAppCollection;
                lstApps.ItemsSource = _sppidAppCollection;
            }
        }

        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            if (lstProjects.SelectedItem != null)
            {
                SPPIDProject sp =  (SPPIDProject)lstProjects.SelectedItem;

                //Close existing apps
                Process p = new Process();
                p.StartInfo.FileName = "CheckRunning.exe";
                p.Start();
                p.WaitForExit(1000 * 60); //* 1);

                //Set ini file settings
                IniFile ini = new IniFile("C:\\Users\\fre11896\\SmartPlantManager.INI");
                ini.IniWriteValue("SmartPlant Manager", "SiteServer", sp.IniFile);
                ini.IniWriteValue("SmartPlant Manager", "ActivePlant", sp.PlantName);
                ini.IniWriteValue("SmartPlant Manager", "ActiveParentPlant", sp.PlantName);

                //Open apps
                foreach (SPPIDApp sa in _sppidAppCollection)
                {
                    if (sa.IsChecked)
                    {
                        p = new Process();
                        p.StartInfo.FileName = sa.Exe;
                        p.Start();
                    }
                }
            }
        }
    }
}
