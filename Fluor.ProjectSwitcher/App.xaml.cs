﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using Fluor.ProjectSwitcher.Class;
using System.Windows.Shell;
using System.Reflection;
using System.Text;

namespace Fluor.ProjectSwitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private string PreselectedProject;

        protected override void OnStartup(StartupEventArgs e)
        {
            PreselectedProject ="";

            // Check if a project has been pre-selected via the jumplist menu
            // Get the project name
            if (e.Args.Count() > 0)
            {
                PreselectedProject = (CombineArgs(e)); // e.Args[0].ToString();
            }

            Messenger.Default.Register<NotificationMessageAction<string>>(this, GetPreselectedProject);
        }

        private static string CombineArgs(StartupEventArgs e)
        {
            // Arguments passed in with a space get split into separate arguments
            // I.e. "Daelim Satellite" becomes [0]Daelim, [1]Satellite
            // These need to be re-combined
            StringBuilder sb = new StringBuilder();

            foreach (var arg in e.Args)
            {
                sb = sb.Append(arg);
                sb = sb.Append(" ");
            }
            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Callback message from VM_Tiles to get the preselected project name.
        /// </summary>
        /// <param name="msg">The callback message.</param>
        private void GetPreselectedProject(NotificationMessageAction<string> msg)
        {
            msg.Execute(PreselectedProject);
        }

        private void tile_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid grid = (Grid)sender;
            //Tile tile = (Tile)sender;
            Messenger.Default.Send<GenericMessage<Grid>>(new GenericMessage<Grid>(grid));
        }

        private void muProject_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            //TODO The binding is not working correctly here; I'm not sure why. For some reason the
            //     The ContextMenu class is bound to the header property of the MenuItem.
            Class.ContextMenu contextMenu = (Class.ContextMenu)mi.Header;

            Fluor.ProjectSwitcher.Class.Utilities.OpenFolder(contextMenu.Value);
            //Fluor.ProjectSwitcher.Class.Utilities.OpenFolder(mi.CommandParameter.ToString());
        }

        private void tbApplicationName_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            Messenger.Default.Send<GenericMessage<TextBlock>>(new GenericMessage<TextBlock>(tb));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            Messenger.Default.Send<GenericMessage<MenuItem>>(new GenericMessage<MenuItem>(mi));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            SwitcherItem selectedTile = (SwitcherItem)btn.DataContext;
            Messenger.Default.Send<GenericMessage<SwitcherItem>>(new GenericMessage<SwitcherItem>(this, selectedTile));
        }

        private void btnTileMain_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Project selectedProject = btn.DataContext as Project;
            
            if (selectedProject != null)
            {
                Messenger.Default.Send(new Message.M_UpdateSelectedTile(selectedProject, this));
            }
            else
            {
                TopApplication selectedApplication = btn.DataContext as TopApplication;
                Messenger.Default.Send(new Message.M_UpdateSelectedTile(selectedApplication, this));
            }
        }

        // Edit button click on tile
        private void btnTileEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Project selectedTile = (Project)btn.DataContext;
            Messenger.Default.Send(new Message.M_EditTile(selectedTile, this));
            //Messenger.Default.Send<Project>(selectedTile);
        }

        // Ok (save) button on the add new view
        private void btnEditOk_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Project selectedTile = (Project)btn.DataContext;

            //Messenger.Default.Send<Message.MessageSaveChangesToTile>(new Message.MessageSaveChangesToTile(selectedTile, this));
        }

        private void btnAddnewContextMenu_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            SwitcherItem switchItem = (SwitcherItem)btn.DataContext;
            //switchItem.AddContextMenu(new Class.ContextMenu(Class.ContextMenu.TypeEnum.PATH, "", ""));
            Class.ContextMenu cm = new Class.ContextMenu();
            cm.Setup(Class.ContextMenu.ContextMenuTypeEnum.PATH, "", "");
            switchItem.AddContextMenu(cm);
        }

        private void btnAddAssociatedApplication_Click(object sender, RoutedEventArgs e)
        {
            //TODO Don't think this is required anymore
            Button btn = (Button)sender;
            Project project = (Project)btn.DataContext;
            Association association = new Association();
            association.Setup(project.Name, "SPPID"); //, null, null);
            project.Associations.Add(association);
            project.SelectedAssociation = association;
        }

        //private void lstAssociation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ListBox lst = (ListBox)sender;
        //    Project project = (Project)lst.DataContext;

        //    project.SelectedAssociation = (Association)lst.SelectedItem;
        //}
    }
}
