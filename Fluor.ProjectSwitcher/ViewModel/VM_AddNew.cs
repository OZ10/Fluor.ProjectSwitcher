using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Fluor.ProjectSwitcher.Class;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Windows;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class VM_AddNew : ViewModelBase 
    {
        private Project selectedItem;
        public Project SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        private Association selectedAssociation;
        public Association SelectedAssociation
        {
            get
            {
                return selectedAssociation;
            }
            set
            {
                selectedAssociation = value;
                RaisePropertyChanged("SelectedAssociation");
            }
        }

        private ObservableCollection<ListBox> selectedItemCollection;
        public ObservableCollection<ListBox> SelectedItemCollection
        {
            get
            {
                return selectedItemCollection;
            }
            set
            {
                selectedItemCollection = value;
                RaisePropertyChanged("SelectedItemCollection");
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

        public VM_AddNew()
        {
            //Messenger.Default.Register<Message.MessageAddOrEditTile>(this, DisplayTileDetails);
            Messenger.Default.Register<Message.M_AddOrEditTile>(this, ShowProjectDetails);
            Messenger.Default.Register<Message.M_ChangeView>(this, ChangeView);
        }

        private void ShowProjectDetails(Message.M_AddOrEditTile msg)
        {
            if (msg.Sender is VM_Tiles)
            {
                SelectedItem = msg.SelectedTile;

                // If the selected project only has one assoication, select it
                if (SelectedItem.Associations.Count == 1)
                {
                    SelectedItem.SelectedAssociation = SelectedItem.Associations[0];
                }
            }
        }

        public void OkButton_Clicked()
        {
            Messenger.Default.Send<Message.M_AddOrEditTile>(new Message.M_AddOrEditTile(SelectedItem, this));
            //proj.Execute(SelectedItem);
            //Messenger.Default.Send<Message.M_SimpleAction>(new Message.M_SimpleAction(Message.M_SimpleAction.Action.RefreshViews));
        }

        private void ChangeView(Message.M_ChangeView msg)
        {
            if (msg.View == Message.M_ChangeView.ViewToSelect.DisplayAddNewTab)
            {
                IsAddNewTabSelected = true;
            }
            else
            {
                IsAddNewTabSelected = false;
            }
        }
    }
}
