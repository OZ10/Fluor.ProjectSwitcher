using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Ini;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // TODO Add code to deal with applications with only one sub application to run - i.e. notepad
        // TODO Fix bug where selected sub apps will launch even if they are not currently displayed - i.e. even with SPEL selected, Drawing Manager will launch because it's selected by default

        private static string SettingsFileName = "Fluor.ProjectSwitcher.Projects.xml";
        private static string SettingsFileBackupName = "Fluor.ProjectSwitcher.Projects_Backup.xml";
        private static string InstallDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private TopApplication _selectedApplication;

        public ProjectSwitcherSettings ProjectSwitcherSettings { get; set; }

        Project selectedTile;
        public Project SelectedTile
        {   
            get { return selectedTile; }
            set
            {
                selectedTile = value;
                RaisePropertyChanged("SelectedProject");
            }
        }

        private ObservableCollection<MenuItem> contextMenus;
        public ObservableCollection<MenuItem> ContextMenus
        {   
            get { return contextMenus; }
            set
            {
                contextMenus = value;
                RaisePropertyChanged("ContextMenus");
            }
        }

        private ObservableCollection<SwitcherItem> breadcrumbCollection;
        public ObservableCollection<SwitcherItem> BreadcrumbCollection
        {   
            get { return breadcrumbCollection; }
            set
            {
                breadcrumbCollection = value;
                RaisePropertyChanged("BreadcrumbCollection");
            }
        }

        private Visibility mainViewButtonsVisibility;
        public Visibility MainViewButtonsVisibility
        {
            get { return mainViewButtonsVisibility; }
            set
            {
                mainViewButtonsVisibility = value;
                RaisePropertyChanged("MainViewButtonsVisibility");
            }
        }

        private string version;
        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                RaisePropertyChanged("Version");
            }
        }

        //private string statusText;
        //public string StatusText
        //{
        //    get { return statusText; }
        //    set
        //    {
        //        statusText = value;
        //        RaisePropertyChanged("StatusText");
        //    }
        //}

        //private ObservableCollection<string> statusTextCollection;
        //public ObservableCollection<string> StatusTextCollection
        //{
        //    get { return statusTextCollection; }
        //    set
        //    {
        //        statusTextCollection = value;
        //        RaisePropertyChanged("StatusTextCollection");
        //    }
        //}

        /// <summary>
        /// Initializes a new instance of the MainViewModel 
        /// </summary>
        public MainViewModel()
        {
            BreadcrumbCollection = new ObservableCollection<SwitcherItem>();
            MainViewButtonsVisibility = Visibility.Visible;

            version = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Messenger.Default.Register<Message.M_UpdateSelectedTile>(this, ChangeSelectedTile);
            Messenger.Default.Register<GenericMessage<TopApplication>>(this, UpdateSelectedApplication);
            Messenger.Default.Register<Message.M_SettingsHaveBeenChanged>(this, SettingsHaveChanged);
            Messenger.Default.Register<Message.M_ChangeView>(this, ChangeView);
        }

        /// <summary>
        /// Setups the environment.
        /// </summary>
        public void SetupEnvironment()
        {
            try
            {
                ProjectSwitcherSettings = CheckStatusOfSettingsFile();

                PopulateProjectsAndApplications();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //Messenger.Default.Send<Message.MessageStatusUpdate>(new Message.MessageStatusUpdate(Visibility.Visible, "Doh!"));
            }
        }

        private void PopulateProjectsAndApplications()
        {
            Messenger.Default.Send<ObservableCollection<TopApplication>>(ProjectSwitcherSettings.Applications);
            Messenger.Default.Send<ObservableCollection<Project>>(ProjectSwitcherSettings.Projects);
        }

        private ProjectSwitcherSettings CheckStatusOfSettingsFile()
        {
            ProjectSwitcherSettings settings = Serialization.Deserialize(InstallDirectory + SettingsFileName);

            if (File.Exists(InstallDirectory + SettingsFileBackupName))
            {
                ProjectSwitcherSettings settingsBackup = Serialization.Deserialize(InstallDirectory + SettingsFileBackupName);

                if (settingsBackup.UserVersion > settings.UserVersion)
                {
                    //UpdateStatusText("Loaded settings from backup");

                    // Overwrite the settings file with this backup file
                    Serialization.Serialize(settingsBackup, InstallDirectory + SettingsFileName);
                    return settingsBackup;
                }
            }

            //UpdateStatusText("Loaded settings");
            return settings;
        }

        public void LoadSettingsManually(string settingFilePath)
        {
            ProjectSwitcherSettings = Serialization.Deserialize(settingFilePath);
            PopulateProjectsAndApplications();
            ProjectSwitcherSettings.HasBeenChanged = true;
            //UpdateStatusText("Loaded settings user selected settings");
        }

        /// <summary>
        /// Closes any open applications and opens new ones.
        /// </summary>
        public async void CloseAndOpenApplications()
        {
            if (SelectedTile != null)
            {
                // Show status grid which over all controls
                Messenger.Default.Send(new Message.MessageStatusUpdate(Visibility.Visible));

                // Only close open applcations if the selected project is not the active project
                if (SelectedTile.IsActiveProject != true)
                {
                    UpdateStatusBar("closing applications");
                    bool closeResult = await Task.Run(() => CloseApplications());
                }

                Messenger.Default.Send(new Message.M_SimpleAction(Message.M_SimpleAction.Action.ChangeActiveProject));

                SetProjectSpecificSettings();

                UpdateStatusBar("opening applications");

                // Open new applications
                System.Threading.Thread.Sleep(1000);
                bool openResult = await Task.Run(() => OpenApplications());

                // Hide the status grid
                Messenger.Default.Send(new Message.MessageStatusUpdate(Visibility.Hidden));
            }
        }

        public void SetProjectSpecificSettings()
        {
            Project selectedProject = SelectedTile as Project;

            // For all associations associated with the selected project
            if (selectedProject != null && _selectedApplication != null)
            {
                foreach (Parameter parameter in selectedProject.Associations.First<Association>(a => a.Name == _selectedApplication.Name).Parameters)
                {
                    // Determine type
                    if (parameter.Type == Parameter.ParameterTypeEnum.INI)
                    {
                        SetIni(parameter);
                    }
                    else if (parameter.Type == Parameter.ParameterTypeEnum.REG)
                    {
                        SetRegistry(parameter);
                    }
                }
            }

            AddSelectedProjectToJumpList(selectedProject);
        }

        private static void AddSelectedProjectToJumpList(Project selectedProject)
        {
            JumpTask task = new JumpTask
            {
                Title = selectedProject.Name,
                Arguments = selectedProject.Name,
                Description = "Open with " + selectedProject.Name + " selected",
                IconResourcePath = InstallDirectory + "SwitcherIcon_w1.ico", //Assembly.GetEntryAssembly().CodeBase,
                ApplicationPath = Assembly.GetEntryAssembly().CodeBase
            };

            JumpList jl = JumpList.GetJumpList(App.Current);

            if (jl == null)
            {
                jl = new JumpList();
            }

            //jl.JumpItems.Add(task);
            //JumpList.SetJumpList(App.Current, jl);
            JumpList.AddToRecentCategory(task);
            jl.Apply();
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
        /// Closes the applications.
        /// </summary>
        /// <returns></returns>
        public bool CloseApplications()
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
        /// Opens the applications.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        private bool OpenApplications()
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
        private void ChangeSelectedTile(Message.M_UpdateSelectedTile msg)
        {
            // Only change the selectedtile if a project tile has been selected
            // do nothing and keep the previously selected tile if an application tile has been selected

            SelectedTile = msg?.SelectedProject;

            if (SelectedTile != null)
            {
                if (SelectedTile.SubItems.Any() | SelectedTile.Associations.Any())
                {
                    // Change the breadcrumb (title bar) to reflect the newly selected item
                    PopulateBreadCrumb(SelectedTile);
                }
            }
        }

        /// <summary>
        /// Populates the breadcrumb in the title bar.
        /// </summary>
        /// <param name="switcherItem">The switcher item to be added.</param>
        private void PopulateBreadCrumb(SwitcherItem switcherItem)
        {
            //Button btn = new Button();
            //btn.Style = (Style)System.Windows.Application.Current.Resources["MetroBreadCrumbButtonStyle"];
            //btn.DataContext = switcherItem;

            // Check if the breadcrumb collection already has items
            // Method: Add each item one by one to a new temp collection until the item which has been select (passed in) is found
            // Then overwrite the Breadcrumb collection with the temp collection - therefore any item after the selected item will be removed.
            if (BreadcrumbCollection.Any())
            {
                ObservableCollection<SwitcherItem> tempCollection = new ObservableCollection<SwitcherItem>();

                foreach (SwitcherItem s in BreadcrumbCollection)
                {
                    if (s != switcherItem)
                    {
                        tempCollection.Add(s);
                    }
                    else
                    {
                        break;
                    }
                }

                BreadcrumbCollection = tempCollection;
            }

            BreadcrumbCollection.Add(switcherItem);
        }

        public void ResetBreadCrumb()
        {
            BreadcrumbCollection = new ObservableCollection<SwitcherItem>();
        }

        ///// <summary>
        ///// Gather & returns all context menus associated with the selected item.
        ///// </summary>
        ///// <param name="getContextMenuMessage">The get context menu message.</param>
        //private void GetContextMenuParameters(NotificationMessageAction<string> msg)
        //{
        //    // Message sent by either the projects or the applications view

        //    string contextMenus = "";

        //    Project project = msg.Sender as Project;

        //    if (project != null)
        //    {
        //        // Get all associtions associated with the selected item
        //        foreach (Association association in project?.Associations) //.Where(ass => ass.ProjectName == msg.Notification))
        //        {
        //            if (association.ContextMenuCollection != null)
        //            {
        //                foreach (Class.ContextMenu contextMenu in association.ContextMenuCollection)
        //                {
        //                    project.AddContextMenu(contextMenu);
        //                }
        //            }

        //            //contextMenus = SetContextMenuValue(contextMenus, association.ContextMenus);
        //        }
        //    }

        //    // Return contextmenus to sender
        //    //TODO This no longer needs to be an Action message
        //    msg.Execute(contextMenus);
        //}

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

        private void UpdateSelectedApplication(GenericMessage<TopApplication> message)
        {
            _selectedApplication = (TopApplication)message.Content;
            Messenger.Default.Send<Message.M_ChangeView>(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayApplicationsTab));
        }

        public void SaveAndClose()
        {
            if (ProjectSwitcherSettings.HasBeenChanged)
            {
                ProjectSwitcherSettings.UserVersion += 1;
                ProjectSwitcherSettings.HasBeenChanged = false;

                Serialization.Serialize(ProjectSwitcherSettings, InstallDirectory + SettingsFileName);

                // Backup file
                File.Copy(SettingsFileName, SettingsFileBackupName, true);
            }
        }

        private void ChangeView(Message.M_ChangeView msg)
        {
            // Change view message - hide the switcher button if the Add New view is shown
            if (msg.View == Message.M_ChangeView.ViewToSelect.DisplayAddNewTab | msg.View == Message.M_ChangeView.ViewToSelect.DisplayEditApplicationsTab)
            {
                MainViewButtonsVisibility = Visibility.Collapsed;
            }
            else
            {
                MainViewButtonsVisibility = Visibility.Visible;
            }
        }

        //private void UpdateStatusText(string message)
        //{
        //    StatusText = message;
        //    if (StatusTextCollection == null) StatusTextCollection = new ObservableCollection<string>();
        //    StatusTextCollection.Add(message);
        //}

        private void SettingsHaveChanged(Message.M_SettingsHaveBeenChanged msg)
        {
            if (ProjectSwitcherSettings != null)
            {
                ProjectSwitcherSettings.HasBeenChanged = msg.SettingsHaveChanged;
            }
        }

        private void UpdateStatusBar(string message)
        {
            Messenger.Default.Send(new Message.MessageStatusUpdate(Visibility.Visible, message));
        }
    }
}