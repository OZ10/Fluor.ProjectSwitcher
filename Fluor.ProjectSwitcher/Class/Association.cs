using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fluor.ProjectSwitcher.Class
{
    public class Association : SwitcherItem 
    {
        public string ProjectName { get; set; }
        public string ApplicationName { get; set; }

        private ObservableCollection<Parameter> _parameters;
        public ObservableCollection<Parameter> Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
                RaisePropertyChanged("Parameters");
            }
        }
        //public string ContextMenus { get; set; }

        public Association(string projectName, string applicationName, IEnumerable<XElement> projectParameters, IEnumerable<XElement> contextMenus)
        {
            ProjectName = projectName;
            ApplicationName = applicationName;
            
            GetContextMenus(contextMenus);
            GetParameters(projectParameters);
        }

        private void GetParameters(IEnumerable<XElement> projectParameters)
        {
            Parameters = new ObservableCollection<Parameter>();

            foreach (XElement parameter in projectParameters)
            {
                Parameter p = new Parameter((Parameter.TypeEnum)Enum.Parse(typeof(Parameter.TypeEnum), parameter.Attribute("TYPE").Value), parameter.Attribute("VALUE").Value, parameter.Attribute("PATH").Value);
                Parameters.Add(p);
            }
        }
    }
}
