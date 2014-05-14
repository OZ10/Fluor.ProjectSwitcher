﻿using System.Windows;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using System;

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
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
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
    }
}