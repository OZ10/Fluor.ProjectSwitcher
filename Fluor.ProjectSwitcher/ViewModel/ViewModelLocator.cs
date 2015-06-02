/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Fluor.ProjectSwitcher"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Fluor.ProjectSwitcher.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator 
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<VM_Tiles>();
            SimpleIoc.Default.Register<VM_Applications>();
            SimpleIoc.Default.Register<VM_Edit>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public VM_Tiles Projects
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VM_Tiles>();
            }
        }

        public VM_Applications Applications
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VM_Applications>();
            }
        }

        public VM_Edit AddNew
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VM_Edit>();
            }
        }
       
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}