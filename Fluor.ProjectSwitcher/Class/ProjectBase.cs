using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Fluor.ProjectSwitcher.Class
{
    public class ProjectBase : ViewModelBase 
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public bool IsActiveProject { get; set; }
        public string Parameters { get; set; }
        public List<string> ApplicationsList { get; set; }
        public ObservableCollection<ProjectBase> SubProjects { get; set; }

        private string applications;
        public string Applications
        {
            get
            {
                return applications;
            }
            set
            {
                applications = value;
                string[] apps = applications.Split(',');

                foreach (var app in apps)
                {
                    ;
                    ApplicationsList.Add(app);
                }
            }
        }

        public ProjectBase()
        {
            ApplicationsList = new List<string>();
            SubProjects = new ObservableCollection<ProjectBase>();
        }

        public void SetProjectSpecificSettings()
        {
            // Method:
            // Split parameters from Parameters property
            // Determine what type of parameters there are
            // Set parameters

            // Split out parameters
            string[] parameters = Parameters.Split(';');

            // For each parameter, determine type and set
            foreach (string parameter in parameters)
            {
                // Determine type
                if (parameter.StartsWith("(INI)"))
                {
                    SetIni(parameter);
                }
            }
        }

        private void SetIni(string parameter)
        {
            Ini.IniFile ini;

            // Split parameter into ini file details
            string[] iniDetails = parameter.Split(':');

            // Array:
            // [0] = Ini file location
            // [X] = Ini file settings
            iniDetails[0] = iniDetails[0].Replace("(INI)", "");
            if (iniDetails[0].Contains("%"))
            {
                ini = new Ini.IniFile(Environment.ExpandEnvironmentVariables(iniDetails[0]));
            }
            else
            {
                ini = new Ini.IniFile(iniDetails[0]);
            }

            // Split ini file settings
            string[] iniSettings = iniDetails[1].Split(new string[] { ")(" }, StringSplitOptions.None);

            foreach (string iniSetting in iniSettings)
            {
                // Array
                // [0] = Section
                // [1] = Key
                // [2] = Value
                string[] setting = iniSetting.Split(',');
                ini.IniWriteValue(setting[0].TrimStart('('), setting[1], setting[2].TrimEnd(')'));
            }
        }
    }
}
