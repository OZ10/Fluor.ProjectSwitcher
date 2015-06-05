using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher.View
{
    /// <summary>
    /// Interaction logic for ApplicationsUC.xaml
    /// </summary>
    public partial class V_Edit : UserControl
    {
        public ViewModel.VM_Edit vm
        {
            get
            {
                return (ViewModel.VM_Edit)DataContext;
            }
        }

        public V_Edit()
        {
            InitializeComponent();
        }

        private void btnEditOk_Click(object sender, RoutedEventArgs e)
        {
            //Messenger.Default.Send<Message.M_SimpleAction>(new Message.M_SimpleAction(Message.M_SimpleAction.Action.DisplayTilesTab));
            //Messenger.Default.Send<Message.M_SimpleAction>(new Message.M_SimpleAction(Message.M_SimpleAction.Action.RefreshViews));
            //Messenger.Default.Send<Message.MessageSaveChangesToTile>(new Message.MessageSaveChangesToTile(null, this));
            vm.OkButton_Clicked();
        }

        private void btnAddnewContextMenu_Click(object sender, RoutedEventArgs e)
        {
            vm.AddNewContextMenu();
        }

        private void btnAddnewAssociation_Click(object sender, RoutedEventArgs e)
        {
            vm.EditAssociation(true, null);
        }

        private void ChangeListViewItem_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            Class.ContextMenu cm = b.DataContext as Class.ContextMenu;

            if (cm != null)
            {
                switch (b.Tag.ToString())
                {
                    case "Edit":
                        vm.SelectedContextMenu = cm;
                        break;
                    case "Delete":
                        vm.DeleteContextMenu(cm);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Class.Association association = b.DataContext as Class.Association;

                if (association != null)
                {
                    switch (b.Tag.ToString())
                    {
                        case "Edit":
                            //vm.SelectedAssociation = association;
                            vm.EditAssociation(false, association);
                            break;
                        case "Delete":
                            vm.DeleteAssociation(association);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void btnCopyFromExistingAssociation_Click(object sender, RoutedEventArgs e)
        {
            vm.CopyParametersFromExisingAssociation();
        }

        private void btnAddNewParameter_Click(object sender, RoutedEventArgs e)
        {
            vm.AddNewParameter();
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            vm.CancelButton_Clicked();
        }

        private void btnDeleteProject_Click(object sender, RoutedEventArgs e)
        {
            vm.DeleteProjectButton_Clicked();
        }

        private void btnAddNewContextMenu_Click_1(object sender, RoutedEventArgs e)
        {
            vm.AddNewAssociationContextMenu();
        }

        //private void muApplication_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem mi = (MenuItem)sender;

        //    Utilities.OpenFolder(mi.CommandParameter.ToString());
        //}
    }
}
