using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Fluor.SPPID.ProjectSwitcher
{
    public class ApplicationListViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ApplicationTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }
        public DataTemplate HeaderTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            SPPIDApp sppidapp = (SPPIDApp)item;

            if (sppidapp.ParentApp == "SEP")
            {
                return SeparatorTemplate;
            }
            else if (sppidapp.ParentApp == "HEADER")
            {
                return HeaderTemplate;
            }
            return ApplicationTemplate;
        }
    }
}
