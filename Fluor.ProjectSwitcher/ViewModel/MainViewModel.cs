using GalaSoft.MvvmLight;
using Ini;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using Fluor.ProjectSwitcher.Class;
using System.ComponentModel;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        // TODO Allow associations -- in one xml row -- between mulitple projects and one application. Required for applications which don't require project specific setup
        //      i.e. notepad is just an exe. Defining a row per project is pointless and messy
        // TODO Create transitions between project & subproject (currently both are on the same tab)
        // TODO Add code to deal with applications with only one sub application to run - i.e. notepad
        // TODO Fix bug where selected sub apps will launch even if they are not currently displayed - i.e. even with SPEL selected, Drawing Manager will launch because it's selected by default

        private XElement _xmlSettings;
        private TopApplication _selectedApplication;

        //public ObservableCollection<Project> ProjectsCollection { get; set; }
        //public ObservableCollection<SwitcherItem> ApplicationsCollection { get; set; }
        //public List<Association> Associations { get; set; }
        //public ObservableCollection<SwitcherItem> AssociatedApplicationCollection { get; set; }

        Project selectedTile;
        public Project SelectedTile
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

        bool isAddNewTabSelected;
        public bool IsAddNewTabSelected
        {
            get
            {
                return isAddNewTabSelected;
            }
            set
            {
                isAddNewTabSelected = value;
                RaisePropertyChanged("IsAddNewTabSelected");
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

            Messenger.Default.Register<Message.MessageUpdateSelectedTile>(this, ChangeSelectedTile);
            Messenger.Default.Register<NotificationMessageAction<string>>(this, GetContextMenuParameters);
            Messenger.Default.Register<GenericMessage<TopApplication>>(this, UpdateApplicationsCollection);
            Messenger.Default.Register<GenericMessage<ObservableCollection<MenuItem>>>(this, UpdateSelectedProjectContextMenus);
            Messenger.Default.Register<GenericMessage<SwitcherItem>>(this, GoToTilesView);
            Messenger.Default.Register<Message.MessageCreateOrEditTile>(this, CreateOrEditTile);
            Messenger.Default.Register<Message.MessageSaveChangesToTile>(this, SavesChangesToSettings);
        }

        /// <summary>
        /// Setups the environment.
        /// </summary>
        public void SetupEnvironment()
        {
            try
            {
                _xmlSettings = XElement.Load("Fluor.ProjectSwitcher.Projects.xml");
                Messenger.Default.Send<Message.M_LoadFromSettings>(new Message.M_LoadFromSettings(Message.M_LoadFromSettings.SettingLoadType.Project, _xmlSettings));
                Messenger.Default.Send<Message.M_LoadFromSettings>(new Message.M_LoadFromSettings(Message.M_LoadFromSettings.SettingLoadType.Application, _xmlSettings));

                //PopulateProjects();
                //PopulateAssociations();
                //PopulateApplications();
            }
            catch (Exception)
            {
                MessageBox.Show("Doh!");
                //Messenger.Default.Send<Message.MessageStatusUpdate>(new Message.MessageStatusUpdate(Visibility.Visible, "Doh!"));
            }
        }

        ///// <summary>
        ///// Reads the project details from the xml file and populates the projects listview.
        ///// </summary>
        ///// <param name="xmlDoc">The XML document.</param>
        //private void PopulateProjects()
        //{
        //    try
        //    {
        //        ProjectsCollection = new ObservableCollection<Project>();

        //        Project project;
        //        foreach (XElement xmlProject in _xmlSettings.Elements("PROJECTS").Elements("PROJECT"))
        //        {
        //            //foreach (XElement xmlProject in xmlProjects.Elements("PROJECT"))
        //            //{
        //                // Create a new project instance
        //                project = new Project(xmlProject.Attribute("NAME").Value,
        //                                        xmlProject.Elements("CONTEXTMENUS").Elements("CONTEXTMENU"),
        //                                        xmlProject.Attribute("MISCTEXT").Value,
        //                                        true,
        //                                        null,
        //                                        false);

        //                // Get any associations associated with this project
        //                project.GetAssociations(project, _xmlSettings);

        //                // Get any sub projects associated with this project
        //                project.CreateSubProjects(xmlProject, _xmlSettings); //, project.ContextMenus);

        //                ProjectsCollection.Add(project);
        //                //ProjectsCollection.Add(new Project("","", false,"","", false));
        //            //}
        //        }

        //        // Pass the projects collection to the Projects View
        //        Messenger.Default.Send(new Message.MessagePopulateProjects(ProjectsCollection));
        //    }
        //    catch (NullReferenceException)
        //    {
        //        MessageBox.Show("The SPPIDProjects section of the configuration XML file contains errors.\n\nMandatory attributes are:\nNAME\nPLANTNAME\nINIFILE\nPIDPATH\nSPENGPATH",
        //            "XML Errors", MessageBoxButton.OK, MessageBoxImage.Stop);
        //        throw;
        //    }
        //}

        /// <summary>
        /// Reads the association details from the xml file and populates the associations collection.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        //private void PopulateAssociations()
        //{
        //    Associations = new List<Association>();
        //    AssociatedApplicationCollection = new ObservableCollection<SwitcherItem>();

        //    Association association;
        //    foreach (XElement xmlAssociations in _xmlSettings.Elements("ASSOCIATIONS"))
        //    {
        //        foreach (XElement xmlAssociation in xmlAssociations.Elements("ASSOCIATION"))
        //        {
        //            association = new Association(xmlAssociation.Attribute("PROJECTNAME").Value, xmlAssociation.Attribute("APPLICATIONNAME").Value,
        //                                            xmlAssociation.Elements("PARAMETERS").Elements("PARAMETER"), xmlAssociation.Elements("CONTEXTMENUS").Elements("CONTEXTMENU"));

        //            Associations.Add(association);
        //        }
        //    }   
        //}

        ///// <summary>
        ///// Reads the application details from the xml file and populates the applications collection.
        ///// </summary>
        ///// <param name="xmlDoc">The XML document.</param>
        //private void PopulateApplications()
        //{
        //    ApplicationsCollection = new ObservableCollection<SwitcherItem>();

        //    TopApplication application;
        //    foreach (XElement xmlApplication in _xmlSettings.Elements("APPLICATION"))
        //    {
        //        application = new TopApplication(xmlApplication.Attribute("NAME").Value, xmlApplication.Elements("CONTEXTMENUS").Elements("CONTEXTMENU"), true);

        //        application.GetSubApplications(xmlApplication, null); //"", application.ContextMenus);

        //        ApplicationsCollection.Add(application);
        //    }
        //}

        /// <summary>
        /// Closes any open applications and opens new ones.
        /// </summary>
        public async void CloseAndOpenApplications()
        {
            if (SelectedTile != null)
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
                //ChangeActiveProject();


                SetProjectSpecificSettings();


                // Update status window
                Messenger.Default.Send(new Message.MessageStatusUpdate(Visibility.Visible, "opening applications"));

                // Open new applications
                System.Threading.Thread.Sleep(1000);
                bool openResult = await OpenApplicationsAsync();

                // Hide the status grid
                Messenger.Default.Send(new Message.MessageStatusUpdate(Visibility.Hidden));
            }
        }

        ///// <summary>
        ///// Changes the active project.
        ///// </summary>
        //private void ChangeActiveProject()
        //{
        //    if (SelectedTile.IsActiveProject == false)
        //    {
        //        SelectedTile.IsActiveProject = true;

        //        // Unselect all other projects
        //        foreach (Project project in ProjectsCollection.Where(proj => proj.Name != SelectedTile.Name))
        //        {
        //            project.IsActiveProject = false;
        //            project.ChangeIsActiveForSubProjects(SelectedTile.Name);
        //        }

        //        // Selected project is now active so set project specific settings for each selected application
        //        SetProjectSpecificSettings();
        //    }
        //}

        public void SetProjectSpecificSettings()
        {
            Project selectedProject = SelectedTile as Project;

            // For all associations associated with the selected project
            //foreach (Association association in Associations.Where(ass => ass.ProjectName == SelectedTile.Name))
            //{
                // Associations have a 'parameters' field. This field contains project & application specific settings which need to be set.
                // This could be an ini file setting(s), registry setting(s) or something else (as long as code as been written to deal with it).

                // For each parameter, determine type and set
                //foreach (Parameter parameter in association.Parameters)
            if (selectedProject != null && _selectedApplication != null)
            {
                foreach (Parameter parameter in selectedProject.Associations.First<Association>(a => a.ApplicationName == _selectedApplication.Name).Parameters)
                {
                    // Determine type
                    if (parameter.Type == Parameter.TypeEnum.INI)
                    {
                        SetIni(parameter);
                    }
                    else if (parameter.Type == Parameter.TypeEnum.REG)
                    {
                        SetRegistry(parameter);
                    }
                }
            }
                
            //}
        }

        private void SetIni(Parameter parameter)
        {
            //TODO Check if ini file exists

            Ini.IniFile ini = new IniFile("");

            ini = new Ini.IniFile(parameter.Path);

            List<string> iniSettings = parameter.GetSettings();

            ini.IniWriteValue(iniSettings[0], iniSettings[1], iniSettings[2]);
        }

        private void SetRegistry(Parameter parameter)
        {
            List<string> regSettings = parameter.GetSettings();

            Registry.SetValue(regSettings[0], regSettings[1], regSettings[2]);
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
            foreach (TopApplication application in SelectedTile.Applications)
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
                foreach (TopApplication application in SelectedTile.Applications)
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
        private void ChangeSelectedTile(Message.MessageUpdateSelectedTile msg)
        {
            // Collect all the applications that are associated with the newly selected item

            SelectedTile = msg.SelectedTile;

            //if (msg.Sender is App | msg.Sender is ViewModelTiles)
            //{
            //    IsAddNewTabSelected = true;
            //    Messenger.Default.Send<Message.MessageUpdateSelectedTile>(new Message.MessageUpdateSelectedTile(SelectedTile, this));
            //}
            if (msg.Sender is ViewModelTiles)
            {
                //TODO Double message here so added this selected tab check. Must be a better way to do this
                if (IsAddNewTabSelected == false)
                {
                    // A tile has been selected so display the tile tab
                    IsTileTabSelected = true;

                    if (SelectedTile != null)
                    {
                        // Change the breadcrumb (title bar) to reflect the newly selected item
                        PopulateBreadCrumb(SelectedTile);

                        // Get all the associations associated with the selected item
                        Messenger.Default.Send<Message.M_GetAssociatedApplications>(new Message.M_GetAssociatedApplications(SelectedTile));

                        // Get all the associations associated with the selected item
                        //foreach (Association association in SelectedTile.Associations) //.Where(ass => ass.ProjectName == SelectedTile.Name))
                        //{
                        //    foreach (TopApplication application in ApplicationsCollection.Where(app => app.Name == association.ApplicationName))
                        //    {
                        //        AssociatedApplicationCollection.Add(application);
                        //    }
                        //}

                        //if (AssociatedApplicationCollection.Any())
                        //{
                        //    if (AssociatedApplicationCollection.Count == 1)
                        //    {
                        //        // Only one application is associated with the selected item. Pass the application details to the applications tab for display.
                        //        TopApplication application = AssociatedApplicationCollection[0] as TopApplication;
                        //        Messenger.Default.Send<GenericMessage<TopApplication>>(new GenericMessage<TopApplication>(application));
                        //    }
                        //    else
                        //    {
                        //        // Send the associated applications to the Tile view
                        //        Messenger.Default.Send(new Message.MessagePopulateApplications(AssociatedApplicationCollection));
                        //    }
                        //}

                        if (SelectedTile.Applications.Any())
                        {
                            if (SelectedTile.Applications.Count == 1)
                            {
                                // Only one application is associated with the selected item. Pass the application details to the applications tab for display.
                                TopApplication application = SelectedTile.Applications[0] as TopApplication;
                                Messenger.Default.Send<GenericMessage<TopApplication>>(new GenericMessage<TopApplication>(application));
                            }
                            else
                            {
                                // Send a message to the Tile view to display associated applications as tiles
                                Messenger.Default.Send<Message.M_SimpleAction>(new Message.M_SimpleAction(Message.M_SimpleAction.Action.DisplayApplicationsAsTiles));
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

            Project project = msg.Sender as Project;
            //contextMenus = SetContextMenuValue(contextMenus, switcherItem.ContextMenus);

            if (project != null)
            {
                // Get all associtions associated with the selected item
                foreach (Association association in project.Associations) //.Where(ass => ass.ProjectName == msg.Notification))
                {
                    foreach (Class.ContextMenu contextMenu in association.ContextMenuCollection)
                    {
                        project.AddContextMenu(contextMenu);
                    }

                    //contextMenus = SetContextMenuValue(contextMenus, association.ContextMenus);
                }
            }

            // Return contextmenus to sender
            //TODO This no longer needs to be an Action message
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
            //Fluor.ProjectSwitcher.Admin.Class.Run adminModule = new Admin.Class.Run(ProjectsCollection, ApplicationsCollection, Associations);
        }

        private void UpdateApplicationsCollection(GenericMessage<TopApplication> message)
        {
            _selectedApplication = (TopApplication)message.Content;
            IsApplicationsTabSelected = true;
        }

        private void UpdateSelectedProjectContextMenus(GenericMessage<ObservableCollection<MenuItem>> msg)
        {
            ContextMenus = msg.Content;
        }

        private void GoToTilesView(GenericMessage<SwitcherItem> msg)
        {
            IsApplicationsTabSelected = false;
            IsAddNewTabSelected = false;
            isTileTabSelected = true;
        }

        private void SavesChangesToSettings(Message.MessageSaveChangesToTile msg)
        {
            //if (msg.Sender is App)
            //{
            //    Project switcherItem = msg.SelectedTile;

            //    if (switcherItem != null)
            //    {
            //        XElement xProject = null;
            //        if (switcherItem.IsNew)
            //        {
            //            xProject = AddNewProjectToXML(switcherItem, xProject);
            //        }
            //        else
            //        {
            //            xProject = FindExistingProject(switcherItem, xProject);
            //        }

            //        UpdateProjectDetails(switcherItem, xProject);

            //        UpdateProjectAssociations(switcherItem);

            //        // Change project name ONLY after searching for association 
            //        switcherItem.OriginalName = switcherItem.Name;
            //    }

            //    // Save changes
            //    _xmlSettings.Save("Fluor.ProjectSwitcher.Projects.xml");
            //    SetupEnvironment();
            //}
            RaisePropertyChanged("ProjectsCollection");
            IsTileTabSelected = true;
        }

        //private void UpdateProjectAssociations(Project switcherItem)
        //{
        //    foreach (XElement xAssociation in _xmlSettings.Elements("ASSOCIATIONS").Elements("ASSOCIATION"))
        //    {
        //        if (xAssociation.Attribute("PROJECTNAME").Value == switcherItem.OriginalName)
        //        {
        //            if (switcherItem.Associations.Any())
        //            {
        //                List<XElement> associations = new List<XElement>();

        //                foreach (Association association in switcherItem.Associations)
        //                {
        //                    SaveContextMenus(association.ContextMenuCollection, xAssociation);
        //                    SaveParameters(association.Parameters, xAssociation);
        //                }
        //                //xAssociation.ReplaceNodes(associations);
        //            }
        //            else
        //            {
        //                // Remove all contextmenu nodes
        //                xAssociation.Element("CONTEXTMENUS").RemoveNodes();
        //            }
        //        }
        //    }
        //}

        //private static void UpdateProjectDetails(Project switcherItem, XElement xProject)
        //{
        //    if (xProject != null)
        //    {
        //        // Rename project if it's changed
        //        if (switcherItem.OriginalName != switcherItem.Name)
        //        {
        //            // Update xml project name
        //            xProject.Attribute("NAME").Value = switcherItem.Name;

        //            // Project has been renamed. Change name on any associations
        //            foreach (Association association in switcherItem.Associations)
        //            {
        //                association.ProjectName = switcherItem.OriginalName;
        //            }
        //        }

        //        SaveContextMenus(switcherItem.ContextMenuCollection, xProject);

        //        xProject.Attribute("MISCTEXT").Value = switcherItem.MiscText;
        //    }
        //}

        //private XElement FindExistingProject(Project switcherItem, XElement xProject)
        //{
        //    // Find existing project
        //    foreach (XElement xExistingProject in _xmlSettings.Elements("PROJECTS").Elements("PROJECT"))
        //    {
        //        //TODO If project has subprojects

        //        if (xExistingProject.Attribute("NAME").Value == switcherItem.OriginalName)
        //        {
        //            xProject = xExistingProject;
        //            break;
        //        }
        //    }
        //    return xProject;
        //}

        ////private XElement AddNewProjectToXML(Project switcherItem, XElement xProject)
        ////{
        ////    // Add new xelement for new project
        ////    xProject = new XElement("PROJECT", new XAttribute("NAME", switcherItem.Name), new XAttribute("MISCTEXT", switcherItem.MiscText));
        ////    _xmlSettings.Element("PROJECTS").Add(xProject);

        ////    if (switcherItem.Associations.Any())
        ////    {
        ////        // Add Association, context menus & parameters nodes
        ////        foreach (Association association in switcherItem.Associations)
        ////        {
        ////            // Create association node
        ////            XElement xAssociation = new XElement("ASSOCIATION", new XAttribute("PROJECTNAME", association.ProjectName), new XAttribute("APPLICATIONNAME", association.ApplicationName));

        ////            // Create contextmenu node
        ////            xAssociation.Add(new XElement("CONTEXTMENUS"));

        ////            // Create parameter node
        ////            xAssociation.Add(new XElement("PARAMETERS"));

        ////            _xmlSettings.Element("ASSOCIATIONS").Add(xAssociation);
        ////        }
        ////    }
        ////    return xProject;
        ////}

        //private static void SaveContextMenus(ObservableCollection<Class.ContextMenu> contextMenuCollection, XElement xmlNode)
        //{
        //    if (contextMenuCollection.Any())
        //    {
        //        List<XElement> contextMenus = new List<XElement>();

        //        foreach (Class.ContextMenu contextMenu in contextMenuCollection)
        //        {
        //            if (contextMenu.Value != "" && contextMenu.DisplayName != "")
        //            {
        //                contextMenus.Add(new XElement("CONTEXTMENU", new XAttribute("TYPE", contextMenu.Type), new XAttribute("VALUE", contextMenu.Value), new XAttribute("DISPLAYNAME", contextMenu.DisplayName)));
        //            }
        //        }
        //        if (xmlNode.Element("CONTEXTMENUS") == null)
        //        {
        //            xmlNode.Add(new XElement("CONTEXTMENUS"));
        //        }

        //        xmlNode.Element("CONTEXTMENUS").ReplaceNodes(contextMenus);
        //    }
        //    else
        //    {
        //        // Remove all contextmenu nodes
        //        xmlNode.Element("CONTEXTMENUS").RemoveNodes();
        //    }
        //}

        //private static void SaveParameters(ObservableCollection<Parameter> parameterCollection, XElement xmlNode)
        //{
        //    if (parameterCollection.Any())
        //    {
        //        List<XElement> parameters = new List<XElement>();

        //        foreach (Parameter parameter in parameterCollection)
        //        {
        //            if (parameter.Value != "" && parameter.Path != "")
        //            {
        //                parameters.Add(new XElement("PARAMETER", new XAttribute("TYPE", parameter.Type), new XAttribute("VALUE", parameter.Value), new XAttribute("PATH", parameter.Path)));
        //            }
        //        }
        //        xmlNode.Element("PARAMETERS").ReplaceNodes(parameters);
        //    }
        //    else
        //    {
        //        // Remove all contextmenu nodes
        //        xmlNode.Element("PARAMETERS").RemoveNodes();
        //    }
        //}

        private void CreateOrEditTile(Message.MessageCreateOrEditTile msg)
        {
            if (msg.Sender is App | msg.Sender is ViewModelTiles)
            {
                SelectedTile = msg.SelectedTile;
                IsAddNewTabSelected = true;
                Messenger.Default.Send<Message.MessageCreateOrEditTile>(new Message.MessageCreateOrEditTile(SelectedTile, this, null ));
            }
        }
    }
}