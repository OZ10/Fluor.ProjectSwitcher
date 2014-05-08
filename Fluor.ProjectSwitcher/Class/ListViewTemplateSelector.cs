using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher
{
    public class ListViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProjectTemplate { get; set; }
        public DataTemplate ApplicationTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }
        public DataTemplate HeaderTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            SPPIDProject sppidProject = item as SPPIDProject;

            if (sppidProject != null)
            {
                if (sppidProject.PlantName == "SEP")
                {
                    return SeparatorTemplate;
                }
                else if (sppidProject.PlantName == "HEADER")
                {
                    return HeaderTemplate;
                }
                return ProjectTemplate;
            }

            SPPIDApp sppidApp = item as SPPIDApp;

            if (sppidApp != null)
            {
                if (sppidApp.ParentApp == "SEP")
                {
                    return SeparatorTemplate;
                }
                else if (sppidApp.ParentApp == "HEADER")
                {
                    return HeaderTemplate;
                }
                return ApplicationTemplate;
            }
            return null;
        }
    }
}
