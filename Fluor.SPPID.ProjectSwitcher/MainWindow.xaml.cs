using System.Windows;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;

namespace Fluor.SPPID.ProjectSwitcher
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
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<Message.StatusUpdateMessage>(this, UpdateStatusWindow);

            AboutWindow.tbVersion.Text = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

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
            Process p = new Process();
            p.StartInfo.FileName = "SPPIDProjects.xml";
            p.Start();
        }

        private void miCloseAllApps_Click(object sender, RoutedEventArgs e)
        {
            vm.CloseApplicationsAsync();
        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow.Visibility = Visibility.Visible;
        }

        private void UpdateStatusWindow(Message.StatusUpdateMessage statusUpdateMessage)
        {
            StatusWindow.Visibility = statusUpdateMessage.Visibility;
            StatusWindow.tbStatus.Text = statusUpdateMessage.StatusText;
        }
    }
}
