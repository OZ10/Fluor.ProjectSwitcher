using System.Collections.ObjectModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using Fluor.ProjectSwitcher.Class;

namespace Fluor.ProjectSwitcher.ViewModel
{
    public class ViewModelApplications :ViewModelBase 
    {
        private ObservableCollection<ApplicationBase> applicationsCollection;
        public ObservableCollection<ApplicationBase> ApplicationsCollection
        {
            get
            {
                return applicationsCollection;
            }
            set
            {
                applicationsCollection = value;
                RaisePropertyChanged("ApplicationsCollection");
            }
        }

        public ViewModelApplications()
        {
            Messenger.Default.Register<Message.MessagePopulateApplications>(this, UpdateApplicationsCollection);
        }

        private void UpdateApplicationsCollection(Message.MessagePopulateApplications populateApplicationsMessage)
        {
            ApplicationsCollection = populateApplicationsMessage.ApplicationsCollection;
        }

        public void MenuItemClicked(SubApplication selectedApp)
        {
            foreach (SubApplication subApplication in selectedApp.SubApplications)
            {
                subApplication.IsSelected = selectedApp.IsSelected;
            }
        }

        //public void SelectApplications(bool isChecked, SPPIDApp selectedApp, MenuItem mi = null)
        //{
        //    //foreach (SPPIDApp sppidApp in ApplicationsCollection)
        //    //{
        //    //    //IF APPLICATION IS ENABLED I.E. NOT A SEPARATOR, HEADER OR HIDDEN ITEM
        //    //    if (sppidApp.IsEnabled == true)
        //    //    {
        //    //        //IF A MENUITEM HAS BEEN PASSED IN THEN EITHER THE "SELECTONLYTHIS" OR "SELECTALLEXCEPTTHIS" BUTTON HAS BEEN CLICKED
        //    //        if (mi != null)
        //    //        {
        //    //            if (selectedApp != null)
        //    //            {
        //    //                if (sppidApp.Name == selectedApp.Name)
        //    //                {
        //    //                    //ASSIGN THE OPPOSITE ISCHECKED VALUE
        //    //                    sppidApp.IsChecked = !isChecked;
        //    //                }
        //    //                else
        //    //                {
        //    //                    sppidApp.IsChecked = isChecked;
        //    //                }
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            sppidApp.IsChecked = isChecked;
        //    //        }
        //    //    }
        //    //}
        //}
    }
}
