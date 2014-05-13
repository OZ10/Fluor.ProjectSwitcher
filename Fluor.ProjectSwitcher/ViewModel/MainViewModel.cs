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
using System.Collections.Generic;

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

        // TODO Allow associations -- in one xml row -- between mulitple projects and one application. Required for applications which don't require project specific setup
        //      i.e. notepad is just an exe. Defining a row per project is pointless and messy.
        // TODO Collapse all & expand all options
        // TODO Icons for menu items
        // TODO Seperators needs to be disabled
        // TODO Application context menus - against association or application???

        public ObservableCollection<Project> ProjectsCollection { get; set; }
        public ObservableCollection<ApplicationBase> ApplicationsCollection { get; set; }
        public List<Association> Associations { get; set; }
        public ObservableCollection<ApplicationBase> AssociatedApplicationCollection { get; set; }

        Project selectedProject;
        public Project SelectedProject
        {
            get
            {
                return selectedProject;
            }
            set
            {
                selectedProject = value;
                RaisePropertyChanged("SelectedProject");
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ProjectsCollection = new ObservableCollection<Project>();
            ApplicationsCollection = new ObservableCollection<Class.ApplicationBase>();
            Associations = new List<Association>();
            AssociatedApplicationCollection = new ObservableCollection<ApplicationBase>();

            Messenger.Default.Register<Message.MessageChangeSelectedProject>(this, ChangeSelectedProject);
            Messenger.Default.Register<NotificationMessageAction<string>>(this, GetContextMenuParameters);
        }

        /// <summary>
        /// Setups the environment.
        /// </summary>
        public void SetupEnvironment()
        {
            XElement xmlDoc = XElement.Load("Fluor.ProjectSwitcher.Projects.xml");

            PopulateProjects(xmlDoc);
            PopulateAssociations(xmlDoc);
            PopulateApplications(xmlDoc);
        }

        /// <summary>
        /// Reads the project details from the xml file and populates the projects listview.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        private void PopulateProjects(XElement xmlDoc)
        {
            try
            {
                Project project;
                foreach (XElement xmlProjects in xmlDoc.Elements("PROJECTS"))
                {
                    foreach (XElement xmlProject in xmlProjects.Elements("PROJECT"))
                    {
                        // Create a new project instance
                        project = new Project(xmlProject.Attribute("NAME").Value, xmlProject.Attribute("ID").Value, (bool)xmlProject.Attribute("ISEXPANDED"));

                        // Get any sub projects associated with this project
                        project.GetSubProjects(xmlProject);

                        ProjectsCollection.Add(project);
                    }
                }

                // Pass the projects collection to the Projects View
                Messenger.Default.Send(new Message.MessagePopulateProjects(ProjectsCollection));
            }
            catch (NullReferenceException)
            {
                //TODO Error trapping
                MessageBox.Show("The SPPIDProjects section of the configuration XML file contains errors.\n\nMandatory attributes are:\nNAME\nPLANTNAME\nINIFILE\nPIDPATH\nSPENGPATH",
                    "XML Errors", MessageBoxButton.OK, MessageBoxImage.Stop);
                throw;
            }
        }

        private void PopulateAssociations(XElement xmlDoc)
        {
            Association association;
            foreach (XElement xmlAssociations in xmlDoc.Elements("ASSOCIATIONS"))
            {
                foreach (XElement xmlAssociation in xmlAssociations.Elements("ASSOCIATION"))
                {
                    association = new Association(xmlAssociation.Attribute("PROJECTNAME").Value, xmlAssociation.Attribute("APPLICATIONNAME").Value,
                                                    xmlAssociation.Attribute("PARAMETERS").Value, xmlAssociation.Attribute("CONTEXTMENU").Value);

                    Associations.Add(association);
                }
            }
                
        }

        private void PopulateApplications(XElement xmlDoc)
        {
            ApplicationBase application;
            foreach (XElement xmlApplication in xmlDoc.Elements("APPLICATION"))
            {
                application = new ApplicationBase(xmlApplication.Attribute("NAME").Value);

                application.GetSubApplications(xmlApplication);

                ApplicationsCollection.Add(application);
            }
        }

        /// <summary>
        /// Closes any open applications and opens new ones.
        /// </summary>
        public async void CloseAndOpenApplications()
        {
            // Show status grid which overs all controls
            Messenger.Default.Send(new Message.MessageStatusUpdate(Visibility.Visible));

            // Only close open applcations if the selected project is not the active project
            if (SelectedProject.IsActiveProject != true)
            {
                Messenger.Default.Send(new Message.MessageStatusUpdate(Visibility.Visible, "closing applications"));
                bool closeResult = await CloseApplicationsAsync();
            }

            // Change the active project and setup any project specific settings
            ChangeActiveProject();

            // Update status window
            Messenger.Default.Send(new Message.MessageStatusUpdate(Visibility.Visible, "opening applications"));

            // Open new applications
            System.Threading.Thread.Sleep(1000);
            bool openResult = await OpenApplicationsAsync();

            // Hide the status grid
            Messenger.Default.Send(new Message.MessageStatusUpdate(Visibility.Hidden));
        }

        private void ChangeActiveProject()
        {
            if (SelectedProject.IsActiveProject == false)
            {
                SelectedProject.IsActiveProject = true;

                // Unselect all other projects
                foreach (Project project in ProjectsCollection.Where(proj => proj.Name != SelectedProject.Name))
                {
                    project.IsActiveProject = false;
                    project.ChangeIsActiveForSubProjects(SelectedProject.Name);
                }

                // Selected project is now active so set project specific settings for each selected application
                SetProjectSpecificSettings();
            }
        }

        public void SetProjectSpecificSettings()
        {
            // For all associations associated with the selected project
            foreach (Association association in Associations.Where(ass => ass.ProjectName == SelectedProject.Name))
            {
                // Associations have a 'parameters' field. This field contains project & application specific settings which need to be set.
                // This could be an ini file setting(s), registry setting(s) or something else (as long as code as been written to deal with it).

                // PARAMETER Structure:
                // Parameters are seperated by ';'
                // Within each parameter there is a type (ini, regitry etc) and one or more setting
                // The type and setting(s) are seperated by '#'

                // TYPE Structure:
                // Within the type is the type name in '( )' and a location (of the ini, registry key etc)

                // SETTING(S) Structure:
                // The setting(s) are seperated by ','

                // Method:
                // Split parameters from Parameters property
                // Determine what type of parameters there are
                // Set parameters

                // Split out parameters
                string[] parameters = association.Parameters.Split(';');

                // For each parameter, determine type and set
                foreach (string parameter in parameters)
                {
                    // Determine type
                    if (parameter.StartsWith("(INI)"))
                    {
                        SetIni(parameter);
                    }
                    else if (parameter.StartsWith("(REG)"))
                    {
                        SetRegistry(parameter);
                    }
                }
            }
        }

        private void SetIni(string parameter)
        {
            //TODO Check if ini file exists

            Ini.IniFile ini;

            // Split parameter into ini type & settings
            string[] iniTypeSettings = parameter.Split('#');

            // Array:
            // [0] = Ini type & location
            // [X] = Ini file settings
            // Remove type string to leave ini location
            iniTypeSettings[0] = iniTypeSettings[0].Replace("(INI)", "");

            // Check if location contains a '%' meaning the string contains an enviroment variable -- TODO There is probably a better way to do this check
            if (iniTypeSettings[0].Contains("%"))
            {
                ini = new Ini.IniFile(Environment.ExpandEnvironmentVariables(iniTypeSettings[0]));
            }
            else
            {
                ini = new Ini.IniFile(iniTypeSettings[0]);
            }

            // Groups of ini file settings are seperated by '( )'.
            // Split ini file settings by ')(' - i.e. the finish ')' and start of a new group '('
            string[] iniSettings = iniTypeSettings[1].Split(new string[] { ")(" }, StringSplitOptions.None);

            // Split each group
            foreach (string iniSetting in iniSettings)
            {
                // Array
                // [0] = Section
                // [1] = Key
                // [2] = Value
                // Note the 'trims' to remove left over '(' or ')'
                string[] setting = iniSetting.Split(',');
                ini.IniWriteValue(setting[0].TrimStart('('), setting[1], setting[2].TrimEnd(')'));
            }
        }

        private void SetRegistry(string parameter)
        {
            // Remove type string to leave registry settings
            parameter = parameter.Replace("(REG):", "");

            // Groups of registry settings are seperated by '( )'.
            // Split registry settings by ')(' - i.e. the finish ')' and start of a new group '('
            string[] regSettings = parameter.Split(new string[] { ")(" }, StringSplitOptions.None);

            // Split each group
            foreach (string regSetting in regSettings)
            {
                // Array
                // [0] = Key - i.e. HKEY_LOCAL......
                // [1] = ValueName
                // [2] = Value
                // Note the 'trims' to remove left over '(' or ')'
                string[] setting = regSetting.Split(',');
                Registry.SetValue(setting[0].TrimStart('('), setting[1], setting[2].TrimEnd(')'));
            }
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
            // Check if the application is already open
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
                int i = 0;
                foreach (SubApplication application in AssociatedApplicationCollection)
                {
                    if (application.IsSelected && application.Exe != "")
                    {
                        LaunchApplication(ref i, application);
                    }

                    foreach (SubApplication subApplication in application.SubApplications.Where(subapp => subapp.IsSelected == true))
                    {
                        LaunchApplication(ref i, subApplication);
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

            // Check if application is already open
            Process[] processName = Process.GetProcessesByName(subApplication.Exe.Replace(".exe", ""));

            if (processName.Length == 0)
            {
                p = new Process();
                p.StartInfo.FileName = subApplication.InstallPath + subApplication.Exe;
                p.Start();
                i += 1;
            }
        }

        private void ChangeSelectedProject(Message.MessageChangeSelectedProject changeSelectedProjectMessage)
        {
            // Collect all the applications that are associated with the newly selected project

            AssociatedApplicationCollection = new ObservableCollection<ApplicationBase>();

            SelectedProject = changeSelectedProjectMessage.SelectedProject;

            // Get all the associations assoicated with the selected project
            foreach (Association association in Associations.Where(ass => ass.ProjectName == SelectedProject.Name))
            {
                foreach (ApplicationBase application in ApplicationsCollection.Where(app => app.Name == association.ApplicationName))
                {
                    foreach (ApplicationBase subApplication in application.SubApplications)
                    {
                        AssociatedApplicationCollection.Add(subApplication); 
                    }
                }
            }
            // Send the associated applications to the Applications view
            Messenger.Default.Send(new Message.MessagePopulateApplications(AssociatedApplicationCollection));
        }

        private void GetContextMenuParameters(NotificationMessageAction<string> getContextMenuMessage)
        {
            foreach (Association association in Associations.Where(ass => ass.ProjectName == getContextMenuMessage.Notification))
            {
                getContextMenuMessage.Execute(association.ContextMenus);
            }
        }
    }
}