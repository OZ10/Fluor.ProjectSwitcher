using System.Windows;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using System;
using System.Windows.Controls;
using System.IO;
using System.Windows.Shell;

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
            //SetUpJumplist();
            vm.SetupEnvironment();
        }

        //private void SetUpJumplist()
        //{
        //    JumpList jumpList = JumpList.GetJumpList(App.Current);
        //    jumpList.JumpItems.Clear();
        //    jumpList.Apply();
        //}

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
            vm.CloseApplications();
            flySettings.IsOpen = false;
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

        private void miRefresh_Click(object sender, RoutedEventArgs e)
        {
            vm.SetupEnvironment();
            flySettings.IsOpen = false;
        }

        private void miAdmin_Click(object sender, RoutedEventArgs e)
        {
            //vm.OpenAdminModule();
            Messenger.Default.Send<GenericMessage<bool>>(new GenericMessage<bool>(this, true));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            vm.SaveAndClose();
            this.Close();
        }

        private void muProject_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            Fluor.ProjectSwitcher.Class.Utilities.OpenFolder(mi.CommandParameter.ToString());
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<GenericMessage<Fluor.ProjectSwitcher.Class.SwitcherItem>>(new GenericMessage<Fluor.ProjectSwitcher.Class.SwitcherItem>(this, null));
            vm.ResetBreadCrumb();
        }

        private void miLoadSettingsFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ofd.Filter = "XML Files (*.xml)|*.xml";
            bool? result = ofd.ShowDialog();

            if (result == true)
            {
                vm.LoadSettingsManually(ofd.FileName);
                flySettings.IsOpen = false;
            }
        }

        private void miExportSettingsFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            sfd.Filter = "XML Files (*.xml)|*.xml";
            sfd.FileName = "Fluor.ProjectSwitcher.Projects_Export.xml";
            //sfd.
            bool? result = sfd.ShowDialog();

            if (result == true)
            {
                File.Copy("Fluor.ProjectSwitcher.Projects.xml", sfd.FileName, true);
                flySettings.IsOpen = false;
            }
        }

        private void miResetSize_Click(object sender, RoutedEventArgs e)
        {
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            flySettings.IsOpen = false;
        }
    }
}
