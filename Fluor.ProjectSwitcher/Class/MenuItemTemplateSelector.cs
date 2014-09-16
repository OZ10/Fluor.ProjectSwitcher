using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher
{
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SeparatorTemplate { get; set; }
        public DataTemplate MenuItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            MenuItem menuItem = (MenuItem)item;
            //MenuItem menuItem = item as MenuItem;

            if (menuItem != null) //.Header != "")
            {
                return MenuItemTemplate;
            }
            else
            {
                return SeparatorTemplate;
            }
        }
    }
}
