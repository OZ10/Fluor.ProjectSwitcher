using System.Windows;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using System;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public ViewModel.MainViewModel vm
        {
            get
            {
                return (ViewModel.MainViewModel)DataContext;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<Message.MessageStatusUpdate>(this, UpdateStatusWindow);

            lblVersion.Content = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            
            vm.SetupEnvironment();
        }

        /// <summary>
        /// Handles the Click event of the btnOpenProject control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            vm.CloseAndOpenApplications();   
        }

        /// <summary>
        /// Handles the Click event of the btnSettings control. Opens the sppid projects xml file
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            flySettings.IsOpen = true;
        }

        private void recDisable_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //recDisable.Visibility = System.Windows.Visibility.Hidden;
            flySettings.IsOpen = false;
            flyAbout.IsOpen = false;
        }

        private void flyOut_IsOpenChanged(object sender, EventArgs e)
        {
            MahApps.Metro.Controls.Flyout fly = (MahApps.Metro.Controls.Flyout)sender;

            if (fly.IsOpen == true)
            {
                recDisable.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                recDisable.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void miCloseAllApps_Click(object sender, RoutedEventArgs e)
        {
            vm.CloseApplicationsAsync();
            flySettings.IsOpen = false;
        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            flySettings.IsOpen = false;
            flyAbout.IsOpen = true;
        }

        private void UpdateStatusWindow(Message.MessageStatusUpdate statusUpdateMessage)
        {
            StatusWindow.Visibility = statusUpdateMessage.Visibility;
            StatusWindow.tbStatus.Text = statusUpdateMessage.StatusText;
        }

        private void miViewSettingsFile_Click(object sender, RoutedEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "Fluor.ProjectSwitcher.Projects.xml";
            p.Start();

            flySettings.IsOpen = false;
        }

        private void flyAbout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            flyAbout.IsOpen = false;
        }

        private void miRefresh_Click(object sender, RoutedEventArgs e)
        {
            vm.SetupEnvironment();
            flySettings.IsOpen = false;
        }

        private void miAdmin_Click(object sender, RoutedEventArgs e)
        {
            //Admin.View.Admin_ViewMain main = new Admin.View.Admin_ViewMain();
            //main.Show();
            vm.OpenAdminModule();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            tbiProject.IsSelected = true;
            vm.GoBackToParent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void muProject_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            Fluor.ProjectSwitcher.Base.Class.Utilities.OpenFolder(mi.CommandParameter.ToString());
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<GenericMessage<Fluor.ProjectSwitcher.Base.Class.Project>>(new GenericMessage<Fluor.ProjectSwitcher.Base.Class.Project>(this, null));
        }
    }
}
