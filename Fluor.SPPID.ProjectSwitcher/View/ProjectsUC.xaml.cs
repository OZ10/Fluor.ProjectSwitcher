﻿using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace Fluor.SPPID.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for ProjectsUC.xaml
    /// </summary>
    public partial class ProjectsUC : UserControl
    {
        public ViewModel.ProjectsViewModel vm
        {
            get
            {
                return (ViewModel.ProjectsViewModel)DataContext;
            }
        }

        public ProjectsUC()
        {
            InitializeComponent();
            Messenger.Default.Register<Message.ChangeSelectedProjectMessage>(this, ChangeSelectedProject);
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

            vm.OpenFolder(mi, sppidProject);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the lstProjects control. Changes the plant name label depending on which project is selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void lstProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = (ListView)sender;

            vm.ProjectChanged(lv);
        }

        private void ChangeSelectedProject(Message.ChangeSelectedProjectMessage changeSelectedProjectMessage)
        {
            lstProjects.SelectedItem = changeSelectedProjectMessage.SelectedProject;
        }
    }
}