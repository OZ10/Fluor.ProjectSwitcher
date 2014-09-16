using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Fluor.ProjectSwitcher.Base.Class;

namespace Fluor.ProjectSwitcher
{
    public class ApplicationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ApplicationTemplate { get; set; }
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ProjectSwitcherItem application = (ProjectSwitcherItem)item;
            //MenuItem menuItem = item as MenuItem;

            if (application != null)
            {
                if (application.SubItems.Any())
                {
                    return HeaderTemplate;
                }
                else
                {
                    return ApplicationTemplate;
                }
            }
            else
            {
                return SeparatorTemplate;
            }
        }
    }
}
