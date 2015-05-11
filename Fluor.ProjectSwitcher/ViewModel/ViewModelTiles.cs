﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using Fluor.ProjectSwitcher.Class;
using System.Windows;
using System.Xml.Linq;
using System;
using System.Xml.Serialization;
using System.IO;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class ViewModelTiles : ViewModelBase
    {
        private ObservableCollection<Project> projectsCollection;
        public ObservableCollection<Project> ProjectsCollection
        {
            get
            {
                return projectsCollection;
            }
            set
            {
                projectsCollection = value;
                RaisePropertyChanged("ProjectsCollection");
            }
        }

        private ObservableCollection<Button> topLevelTileCollection;
        public ObservableCollection<Button> TopLevelTileCollection
        {
            get
            {
                return topLevelTileCollection;
            }
            set
            {
                topLevelTileCollection = value;
                RaisePropertyChanged("TopLevelProjectTileCollection");
            }
        }

        private ObservableCollection<Button> activeTileCollection;
        public ObservableCollection<Button> ActiveTileCollection
        {
            get
            {
                return activeTileCollection;
            }
            set
            {
                activeTileCollection = value;
                RaisePropertyChanged("ActiveTileCollection");
            }
        }

        private Project selectedTile;
        public Project SelectedTile
        {
            get
            {
                return selectedTile;
            }
            set
            {
                selectedTile = value;
                RaisePropertyChanged("SelectedTile");

                // Send a message to the main view model containing the newly selected item
                Messenger.Default.Send<Message.MessageUpdateSelectedTile>(new Message.MessageUpdateSelectedTile(selectedTile, this));
            }
        }

        private Visibility isEditMode;
        public Visibility IsEditMode
        {
            get
            {
                return isEditMode;
            }
            set
            {
                isEditMode = value;
                RaisePropertyChanged("IsEditMode");
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelTiles"/> class.
        /// </summary>
        public ViewModelTiles()
        {
            //Messenger.Default.Register<Message.MessagePopulateProjects>(this, UpdateProjectsCollection);
            Messenger.Default.Register<GenericMessage<Grid>>(this, DisplayContextMenus);
            Messenger.Default.Register<GenericMessage<SwitcherItem>>(this, GoBackToParent);
            Messenger.Default.Register<Message.M_SimpleAction>(this, DisplayApplicationsAsTiles);
            Messenger.Default.Register<ObservableCollection<Project>>(this, PopulateProjects);
            Messenger.Default.Register<Project>(this, AddOrEditProject);
            Messenger.Default.Register<Message.M_AddOrEditTile>(this, SaveChangesToProject);
            Messenger.Default.Register<Message.M_ChangeView>(this, ChangeView);

            Messenger.Default.Register<GenericMessage<bool>>(this, SetEditMode);

            IsEditMode = Visibility.Collapsed;
        }

        /// <summary>
        /// Reads the project details from the xml file and populates the projects listview.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        private void PopulateProjects(ObservableCollection<Project> msg)
        {
            try
            {
                ProjectsCollection = msg;

                CreateTiles();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Errooooooooorrrrrr");
                throw;
            }
        }

        /// <summary>
        /// reates a tile for each project.
        /// </summary>
        /// <param name="msg">Message containing a collection of projects.</param>
        private void CreateTiles()
        {
            TopLevelTileCollection = new ObservableCollection<Button>();
            ActiveTileCollection = new ObservableCollection<Button>();

            foreach (Project project in ProjectsCollection)
            {
                Button tile = CreateTile(project);

                TopLevelTileCollection.Add(tile);
            }

            ActiveTileCollection = TopLevelTileCollection;
        }

        /// <summary>
        /// Updates the applications collection and creates a tile for each application.
        /// </summary>
        /// <param name="msg">Message containing a collection of applications.</param>
        private void DisplayApplicationsAsTiles(Message.M_SimpleAction msg)
        {
            //ApplicationsCollection = msg.ApplicationsCollection;

            if (msg.SimpleAction == Message.M_SimpleAction.Action.DisplayApplicationsAsTiles)
            {
                ActiveTileCollection = new ObservableCollection<Button>();

                foreach (TopApplication application in SelectedTile.Applications) //  ApplicationsCollection)
                {
                    Button tile = CreateTile(application);

                    ActiveTileCollection.Add(tile);
                }
            }
        }

        /// <summary>
        /// Creates a tile.
        /// </summary>
        /// <param name="switcherItem">The switcher item (project/application).</param>
        /// <returns></returns>
        private Button CreateTile(SwitcherItem switcherItem)
        {
            //TODO See if style can be set in XMAL
            Button tile = new Button();
            tile.Click += new RoutedEventHandler(Tile_Clicked);
            tile.DataContext = switcherItem;
            tile.Style = (Style)System.Windows.Application.Current.Resources["MetroTileCustom"];

            return tile;
        }

        /// <summary>
        /// Handles the Clicked event of the Tile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Tile_Clicked(object sender, RoutedEventArgs e)
        {
            Button tile = (Button)sender;
            SwitcherItem switcherItem = (SwitcherItem)tile.DataContext;

            // Try to cast the item as a Project
            Project project = switcherItem as Project;

            if (project != null)
            {
                if (project.SubItems.Any())
                {
                    // Reset the active tile collection and add all subitems of the selected tile
                    ActiveTileCollection = new ObservableCollection<Button>();

                    foreach (SwitcherItem subProject in project.SubItems)
                    {
                        Button tvi = CreateTile(subProject);

                        ActiveTileCollection.Add(tvi);
                    }
                }

                SelectedTile = project;

                ChangeActiveProject();

                // TODO Is this message required? switcherItem already has a collection of context menus.
                //GetContextMenus(switcherItem);
                //Messenger.Default.Send<GenericMessage<ObservableCollection<MenuItem>>>(new GenericMessage<ObservableCollection<MenuItem>>(switcherItem.ContextMenuCollection));
            }
            else
            {
                // Selected tile must be an application. Send a message to the main view model containing the selected application
                TopApplication application = switcherItem as TopApplication;
                Messenger.Default.Send<GenericMessage<TopApplication>>(new GenericMessage<TopApplication>(this, application));
            }
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
                //SetProjectSpecificSettings();
            }
        }

        /// <summary>
        /// Displays the context menus. Triggered by a right-click on a tile.
        /// </summary>
        /// <param name="msg">Message containing the grid control contained within tile that has been clicked.</param>
        public void DisplayContextMenus(GenericMessage<Grid> msg)
        {
            Grid grid = (Grid)msg.Content;

            SwitcherItem switcherItem = (SwitcherItem)grid.DataContext;

            GetContextMenus(switcherItem);
            grid.ContextMenu.ItemsSource = switcherItem.ContextMenuCollection;
        }

        /// <summary>
        /// Get the context menus associated with the selected item.
        /// </summary>
        /// <param name="switcherItem">The switcher item.</param>
        private void GetContextMenus(SwitcherItem switcherItem)
        {
            // Send a message containing the switcher item's name to the main view model. The main view model returns the context
            // menu parameters as listed in the associations section
            Messenger.Default.Send(new NotificationMessageAction<string>(switcherItem, switcherItem.Name, (contextMenuParameters) =>
            {
                //switcherItem.CreateContextMenus(contextMenuParameters);
            }));
        }

        /// <summary>
        /// Changes the tile collection to the selected item from the title bar.
        /// </summary>
        /// <param name="msg">Message containing the selected item.</param>
        public void GoBackToParent(GenericMessage<SwitcherItem> msg)
        {
            if (msg.Sender is Fluor.ProjectSwitcher.App | msg.Sender is MainWindow)
            {
                Messenger.Default.Send<Message.M_ChangeView>(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayTilesTab));

                //SwitcherItem switcherItem = (SwitcherItem)msg.Content;
                Project switcherItem = msg.Content as Project;

                if (switcherItem == null)
                {
                    // Selected item had no parent. Reset to top level projects
                    ActiveTileCollection = TopLevelTileCollection;
                    SelectedTile = null;
                }
                else
                {
                    // TODO Sure this can be done in the better way. Think you can link a class to a control template so I wouldn't have to create a new tile object each time.
                    ActiveTileCollection = new ObservableCollection<Button>();

                    // Create a tile for each of the item's subitems
                    foreach (SwitcherItem subProject in switcherItem.SubItems)
                    {
                        Button tile = CreateTile(subProject);
                        ActiveTileCollection.Add(tile);
                    }
                    SelectedTile = switcherItem;
                }
            }
        }

        private void SetEditMode(GenericMessage<bool> msg)
        {
            //TODO This needs to be refactored 
            if (IsEditMode == Visibility.Collapsed)
            {
                IsEditMode = Visibility.Visible;

                foreach (Button tile in ActiveTileCollection)
                {
                    SwitcherItem swi = (SwitcherItem)tile.DataContext;
                    swi.IsEditMode = IsEditMode;
                }
            }
            else
            {
                IsEditMode = Visibility.Collapsed;

                foreach (Button tile in ActiveTileCollection)
                {
                    SwitcherItem swi = (SwitcherItem)tile.DataContext;
                    swi.IsEditMode = IsEditMode;
                }
            }
        }

        private void AddOrEditProject(Project project)
        {
            Messenger.Default.Send<Message.M_ChangeView>(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayAddNewTab));
            Messenger.Default.Send<Message.M_AddOrEditTile>(new Message.M_AddOrEditTile(project, this));
        }

        private void SaveChangesToProject(Message.M_AddOrEditTile msg)
        {
            if (msg.Sender is ViewModelAddNew)
            {
                Project project = msg.SelectedTile;

                if (project.IsNew)
                {
                    ProjectsCollection.Add(project);
                    UpdateTiles();
                }
                Messenger.Default.Send<Message.M_ChangeView>(new Message.M_ChangeView(Message.M_ChangeView.ViewToSelect.DisplayTilesTab));
            }
        }

        private void UpdateTiles()
        {
            foreach (Project project in ProjectsCollection.Where((p) => p.IsNew == true))
            {
                Button tile = CreateTile(project);
                TopLevelTileCollection.Add(tile);
            }
        }

        private void ChangeView(Message.M_ChangeView msg)
        {
            if (msg.View == Message.M_ChangeView.ViewToSelect.DisplayTilesTab)
            {
                IsTileTabSelected = true;
            }
            else
            {
                IsTileTabSelected = false;
            }
        }
    }
}