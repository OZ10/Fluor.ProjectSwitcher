using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Fluor.ProjectSwitcher.Class;
using System.Windows.Shell;
using System.Reflection;
using System.Text;

namespace Fluor.ProjectSwitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ResourceDictionary AppTheme { get { return Resources.MergedDictionaries[2]; } } 

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
            Messenger.Default.Register<GenericMessage<Uri>>(this, ChangeTheme);
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

        private void ChangeTheme(GenericMessage<Uri> msg)
        {
            AppTheme.MergedDictionaries.Clear();
            AppTheme.MergedDictionaries.Add(new ResourceDictionary() { Source = msg.Content });
        }
    }
}
