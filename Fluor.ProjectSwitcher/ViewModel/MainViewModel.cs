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
using System.Collections.Generic;
using Fluor.ProjectSwitcher.Base.Class;
using System.ComponentModel;
using System.Windows.Controls;

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
        //      i.e. notepad is just an exe. Defining a row per project is pointless and messy
        // TODO Create transitions between project & subproject (currently both are on the same tab)
        // TODO Create icons for applications
        // TODO Create close & min/max buttons
        // TODO Add code to deal with applications with only one sub application to run - i.e. notepad
        // TODO Fix bug where selected sub apps will launch even if they are not currently displayed - i.e. even with SPEL selected, Drawing Manager will launch because it's selected by default

        public ObservableCollection<Project> ProjectsCollection { get; set; }
        public ObservableCollection<SwitcherItem> ApplicationsCollection { get; set; }
        public List<Association> Associations { get; set; }
        public ObservableCollection<SwitcherItem> AssociatedApplicationCollection { get; set; }

        SwitcherItem selectedTile;
        public SwitcherItem SelectedTile
        {
            get
            {
                return selectedTile;
            }
            set
            {
                selectedTile = value;
                RaisePropertyChanged("SelectedProject");
            }
        }

        private ObservableCollection<MenuItem> contextMenus;
        public ObservableCollection<MenuItem> ContextMenus
        {
            get
            {
                return contextMenus;
            }
            set
            {
                contextMenus = value;
                RaisePropertyChanged("ContextMenus");
            }
        }

        bool isTileTabSelected;
        public bool IsTileTabSelected
        {
            get
            {
                return isTileTabSelected;
            }
            set
            {
                isTileTabSelected = value;
                RaisePropertyChanged("IsTileTabSelected");
            }
        }

        bool isApplicationsTabSelected;
        public bool IsApplicationsTabSelected
        {
            get
            {
                return isApplicationsTabSelected;
            }
            set
            {
                isApplicationsTabSelected = value;
                RaisePropertyChanged("IsApplicationsTabSelected");
            }
        }

        private ObservableCollection<Button> breadcrumbCollection;
        public ObservableCollection<Button> BreadcrumbCollection
        {
            get
            {
                return breadcrumbCollection;
            }
            set
            {
                breadcrumbCollection = value;
                RaisePropertyChanged("BreadcrumbCollection");
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel 
        /// </summary>
        public MainViewModel()
        {
            IsTileTabSelected = true;
            BreadcrumbCollection = new ObservableCollection<Button>();

            //Messenger.Default.Register<Message.MessageChangeSelectedProject>(this, ChangeSelectedProject);
            Messenger.Default.Register<GenericMessage<SwitcherItem>>(this, ChangeSelectedTile);
            Messenger.Default.Register<NotificationMessageAction<string>>(this, GetContextMenuParameters);
            Messenger.Default.Register<GenericMessage<TopApplication>>(this, UpdateApplicationsCollection);
            Messenger.Default.Register<GenericMessage<ObservableCollection<MenuItem>>>(this, UpdateSelectedProjectContextMenus);
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
                ProjectsCollection = new ObservableCollection<Project>();

                Project project;
                foreach (XElement xmlProjects in xmlDoc.Elements("PROJECTS"))
                {
                    foreach (XElement xmlProject in xmlProjects.Elements("PROJECT"))
                    {
                        // Create a new project instance
                        project = new Project(xmlProject.Attribute("NAME").Value,
                                                xmlProject.Attribute("ID").Value,
                                                (bool)xmlProject.Attribute("ISEXPANDED"),
                                                xmlProject.Attribute("CONTEXTMENU").Value,
                                                xmlProject.Attribute("MISCTEXT").Value,
                                                true,
                                                null);

                        // Get any sub projects associated with this project
                        project.CreateSubProjects(xmlProject, project.ContextMenus);
                        
                        ProjectsCollection.Add(project);
                        //ProjectsCollection.Add(new Project("","", false,"","", false));
                    }
                }

                // Pass the projects collection to the Projects View
                Messenger.Default.Send(new Message.MessagePopulateProjects(ProjectsCollection));
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("The SPPIDProjects section of the configuration XML file contains errors.\n\nMandatory attributes are:\nNAME\nPLANTNAME\nINIFILE\nPIDPATH\nSPENGPATH",
                    "XML Errors", MessageBoxButton.OK, MessageBoxImage.Stop);
                throw;
            }
        }

        /// <summary>
        /// Reads the association details from the xml file and populates the associations collection.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        private void PopulateAssociations(XElement xmlDoc)
        {
            Associations = new List<Association>();
            AssociatedApplicationCollection = new ObservableCollection<SwitcherItem>();

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

        /// <summary>
        /// Reads the application details from the xml file and populates the applications collection.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        private void PopulateApplications(XElement xmlDoc)
        {
            ApplicationsCollection = new ObservableCollection<SwitcherItem>();

            TopApplication application;
            foreach (XElement xmlApplication in xmlDoc.Elements("APPLICATION"))
            {
                application = new TopApplication(xmlApplication.Attribute("NAME").Value, xmlApplication.Attribute("CONTEXTMENU").Value, true);

                application.GetSubApplications(xmlApplication, null); //"", application.ContextMenus);

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
            if (SelectedTile.IsActiveProject != true)
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

        /// <summary>
        /// Changes the active project.
        /// </summary>
        private void ChangeActiveProject()
        {
            if (SelectedTile.IsActiveProject == false)
            {
                SelectedTile.IsActiveProject = true;

                // Unselect all other projects
                foreach (Project project in ProjectsCollection.Where(proj => proj.Name != SelectedTile.Name))
                {
                    project.IsActiveProject = false;
                    project.ChangeIsActiveForSubProjects(SelectedTile.Name);
                }

                // Selected project is now active so set project specific settings for each selected application
                SetProjectSpecificSettings();
            }
        }

        public void SetProjectSpecificSettings()
        {
            // For all associations associated with the selected project
            foreach (Association association in Associations.Where(ass => ass.ProjectName == SelectedTile.Name))
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
                string[] parameters = Parameter.GetParameters(association.Parameters);

                // For each parameter, determine type and set
                foreach (string parameter in parameters)
                {
                    // Determine type
                    if (parameter.StartsWith("(INI)"))
                    {
                        SetIni(association, parameter);
                    }
                    else if (parameter.StartsWith("(REG)"))
                    {
                        SetRegistry(parameter);
                    }
                }
            }
        }

        private void SetIni(Association association, string parameter)
        {
            //TODO Check if ini file exists

            Ini.IniFile ini = new IniFile("");

            // Split parameter into ini type & settings
            List<string> iniTypeSettings = Parameter.GetTypeAndSettings(parameter);

            // Array:
            // [0] = Ini type & location
            // [X] = Ini file settings
            // Remove type string to leave ini location
            //iniTypeSettings[0] = iniTypeSettings[0].Replace("(INI)", "");

            // Check if location contains a '%' meaning the string contains an enviroment variable -- TODO There is probably a better way to do this check
            if (iniTypeSettings[0].Contains("%"))
            {
                ini = new Ini.IniFile(Environment.ExpandEnvironmentVariables(iniTypeSettings[0]));
            }
            else if (iniTypeSettings[0].Contains("*"))
            {
                foreach (TopApplication app in ApplicationsCollection.Where(a => a.Name == association.ApplicationName))
                {
                    SubApplication subApp = (SubApplication)app.SubItems[0];

                    switch (iniTypeSettings[0])
                    {
                        case "*installpath*":
                            ini = new Ini.IniFile(subApp.InstallPath + iniTypeSettings[1]);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                ini = new Ini.IniFile(iniTypeSettings[0]);
            }

            // Groups of ini file settings are seperated by '( )'.
            // Split ini file settings by ')(' - i.e. the finish ')' and start of a new group '('
            List<string> iniSettings = Parameter.GetSettings(iniTypeSettings[1]); //.Split(new string[] { ")(" }, StringSplitOptions.None);

            // Split each group
            foreach (string iniSetting in iniSettings)
            {
                // Array
                // [0] = Section
                // [1] = Key
                // [2] = Value
                List<string> setting = Parameter.GetSettingValues(iniSetting); //.Split(',');
                ini.IniWriteValue(setting[0], setting[1], setting[2]);
            }
        }

        private void SetRegistry(string parameter)
        {
            // Remove type string to leave registry settings
            parameter = parameter.Replace("(REG)#", "");

            // Groups of registry settings are seperated by '( )'.
            // Split registry settings by ')(' - i.e. the finish ')' and start of a new group '('
            List<string> regSettings = Parameter.GetSettings(parameter); //.Split(new string[] { ")(" }, StringSplitOptions.None);

            // Split each group
            foreach (string regSetting in regSettings)
            {
                // Array
                // [0] = Key - i.e. HKEY_LOCAL......
                // [1] = ValueName
                // [2] = Value
                // Note the 'trims' to remove left over '(' or ')'
                List<string> setting = Parameter.GetSettingValues(regSetting); //.Split(',');
                Registry.SetValue(setting[0], setting[1], setting[2]);
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
            foreach (TopApplication application in AssociatedApplicationCollection)
            {
                CloseSubApplications(application);
            }
            return true;
        }

        private static void CloseSubApplications(TopApplication application)
        {
            foreach (SubApplication subApplication in application.SubItems)
            {
                Process[] procs = Process.GetProcessesByName(subApplication.Exe.Replace(".exe", ""));

                if (procs.Length > 0)
                {
                    for (var i = 0; i < procs.Length; i++)
                    {
                        procs[i].CloseMainWindow();
                    }
                }

                if (subApplication.SubItems.Any())
                {
                    CloseSubApplications(subApplication);
                }
            }
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
                foreach (TopApplication application in AssociatedApplicationCollection)
                {
                    foreach (SubApplication subApplication in application.SubItems)
                    {
                        if (subApplication.IsSelected == true && subApplication.Exe != "")
                        {
                            LaunchApplication(ref i, subApplication);
                        }

                        OpeningSubApplications(i, subApplication);
                    }
                    System.Threading.Thread.Sleep(2000 * i);
                }
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Executable is missing from install directory.\n\nCheck the executable paths in the configuration XML file.", "Executable Missing", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
                //throw;
            }
        }

        private void OpeningSubApplications(int i, SubApplication application)
        {
            try
            {
                foreach (SubApplication subApplication in application.SubItems)
                {
                    if (subApplication.IsSelected == true)
                    {
                        LaunchApplication(ref i, subApplication);
                    }

                    if (subApplication.SubItems.Any())
                    {
                        OpeningSubApplications(i, subApplication);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Executable is missing from install directory.\n\nCheck the executable paths in the configuration XML file.", "Executable Missing", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LaunchApplication(ref int i, SubApplication subApplication)
        {
            Process p;

            // Check if application is already open
            Process[] processName = Process.GetProcessesByName(subApplication.Exe.Replace(".exe", ""));

            try
            {
                if (processName.Length == 0)
                {
                    p = new Process();
                    p.StartInfo.FileName = subApplication.InstallPath + subApplication.Exe;
                    p.Start();
                    i += 1;
                }
            }
            catch (Win32Exception)
            {
                MessageBox.Show(subApplication.Name + " could not be started.\n\n- Check that application is installed correctly.\n- Check that the 'install path' in the configuration XML file is valid.", "Check Installation", MessageBoxButton.OK, MessageBoxImage.Error);
                //throw;
            }
        }

        /// <summary>
        /// Changes the selected tile.
        /// </summary>
        /// <param name="msg">Message containing the selected item.</param>
        private void ChangeSelectedTile(GenericMessage<SwitcherItem> msg)
        {
            if (msg.Sender is ViewModelTiles)
            {
                // Collect all the applications that are associated with the newly selected item
                AssociatedApplicationCollection = new ObservableCollection<SwitcherItem>();

                // A tile has been selected so display the tile tab
                IsTileTabSelected = true;

                SelectedTile = (SwitcherItem)msg.Content;

                if (SelectedTile != null)
                {
                    // Change the breadcrumb (title bar) to reflect the newly selected item
                    PopulateBreadCrumb(SelectedTile);

                    // Get all the associations associated with the selected item
                    foreach (Association association in Associations.Where(ass => ass.ProjectName == SelectedTile.Name))
                    {
                        foreach (TopApplication application in ApplicationsCollection.Where(app => app.Name == association.ApplicationName))
                        {
                            AssociatedApplicationCollection.Add(application);
                        }
                    }

                    if (AssociatedApplicationCollection.Any())
                    {
                        if (AssociatedApplicationCollection.Count == 1)
                        {
                            // Only one application is associated with the selected item. Pass the application details to the applications tab for display.
                            TopApplication application = AssociatedApplicationCollection[0] as TopApplication;
                            Messenger.Default.Send<GenericMessage<TopApplication>>(new GenericMessage<TopApplication>(application));
                        }
                        else
                        {
                            // Send the associated applications to the Tile view
                            Messenger.Default.Send(new Message.MessagePopulateApplications(AssociatedApplicationCollection));
                        }
                    }
                }
                else
                {
                    // Selected tile was null (home button selected), reset the breadcrumb collection.
                    BreadcrumbCollection = new ObservableCollection<Button>();
                }
            }
        }

        /// <summary>
        /// Populates the breadcrumb in the title bar.
        /// </summary>
        /// <param name="switcherItem">The switcher item to be added.</param>
        private void PopulateBreadCrumb(SwitcherItem switcherItem)
        {
            Button btn = new Button();
            btn.Style = (Style)System.Windows.Application.Current.Resources["MetroBreadCrumbButtonStyle"];
            btn.DataContext = switcherItem;
            btn.ToolTip = switcherItem.MiscText;

            // Check if the breadcrumb collection already has items
            // Method: Add each item one by one to a new temp collection until the item which has been select (passed in) is found
            // Then overwrite the Breadcrumb collection with the temp collection - therefore any item after the selected item will be removed.
            if (BreadcrumbCollection.Any())
            {
                ObservableCollection<Button> tempCollection = new ObservableCollection<Button>();

                foreach (Button b in BreadcrumbCollection)
                {
                    if (b.DataContext != btn.DataContext)
                    {
                        tempCollection.Add(b);
                    }
                    else
                    {
                        break;
                    }
                }

                BreadcrumbCollection = tempCollection;
            }

            BreadcrumbCollection.Add(btn);
        }

        /// <summary>
        /// Gather & returns all context menus associated with the selected item.
        /// </summary>
        /// <param name="getContextMenuMessage">The get context menu message.</param>
        private void GetContextMenuParameters(NotificationMessageAction<string> msg)
        {
            // Message sent by either the projects or the applications view

            string contextMenus = "";

            SwitcherItem switcherItem = (SwitcherItem)msg.Sender;
            contextMenus = SetContextMenuValue(contextMenus, switcherItem.ContextMenus);

            // Get all associtions associated with the selected item
            foreach (Association association in Associations.Where(ass => ass.ProjectName == msg.Notification))
            {
                contextMenus = SetContextMenuValue(contextMenus, association.ContextMenus);
            }

            // Return contextmenus to sender
            msg.Execute(contextMenus);
        }

        /// <summary>
        /// Combines context menu values (strings) into one string.
        /// </summary>
        /// <param name="contextMenus">Current context menus string.</param>
        /// <param name="value">Value to add.</param>
        /// <returns></returns>
        private string SetContextMenuValue(string contextMenus, string value)
        {
            if (value != "")
            {
                if (contextMenus != "")
                {
                    return contextMenus + ";" + value;
                }
                else
                {
                    return value;
                }
            }
            return contextMenus;
        }

        /// <summary>
        /// Opens the admin module.
        /// </summary>
        public void OpenAdminModule()
        {
            Fluor.ProjectSwitcher.Admin.Class.Run adminModule = new Admin.Class.Run(ProjectsCollection, ApplicationsCollection, Associations);
        }

        private void UpdateApplicationsCollection(GenericMessage<TopApplication> message)
        {
            IsApplicationsTabSelected = true;
        }

        private void UpdateSelectedProjectContextMenus(GenericMessage<ObservableCollection<MenuItem>> msg)
        {
            ContextMenus = msg.Content;
        }
    }
}