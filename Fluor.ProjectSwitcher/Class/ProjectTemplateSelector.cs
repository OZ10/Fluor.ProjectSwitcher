using Fluor.ProjectSwitcher.Base.Class;
using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher
{
    public class ProjectTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SeparatorTemplate { get; set; }
        public HierarchicalDataTemplate ProjectTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Project proj = (Project)item;
            //MenuItem menuItem = item as MenuItem;

            if (proj.Name != "")
            {
                return ProjectTemplate;
            }
            else
            {
                return SeparatorTemplate;
            }
        }
    }
}
