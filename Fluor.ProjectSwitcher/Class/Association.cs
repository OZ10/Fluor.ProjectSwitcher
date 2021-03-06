﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Fluor.ProjectSwitcher.Class
{
    public class Association : SwitcherItem 
    {
        [XmlAttribute()]
        public string ProjectName { get; set; }

        //[XmlAttribute()]
        //public string ApplicationName { get; set; }

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

        public Association()
        {
        }

        public void Setup(string projectName, string applicationName) //, IEnumerable<XElement> projectParameters, IEnumerable<XElement> contextMenus)
        {
            ProjectName = projectName;
            Name = applicationName;

            Parameters = new ObservableCollection<Parameter>();

            //GetContextMenus(contextMenus);
            //GetParameters(projectParameters);
        }

        private void GetParameters(IEnumerable<XElement> projectParameters)
        {
            Parameters = new ObservableCollection<Parameter>();

            if (projectParameters != null)
            {
                foreach (XElement parameter in projectParameters)
                {
                    Parameter p = new Parameter();
                    p.Setup((Parameter.ParameterTypeEnum)Enum.Parse(typeof(Parameter.ParameterTypeEnum), parameter.Attribute("TYPE").Value), parameter.Attribute("VALUE").Value, parameter.Attribute("PATH").Value);
                    Parameters.Add(p);
                }
            }
            else
            {
                // Add a blank row so user can input data
                Parameter p = new Parameter();
                p.Setup(Parameter.ParameterTypeEnum.INI, "", "");
                Parameters.Add(p);
            }
        }
    }
}
