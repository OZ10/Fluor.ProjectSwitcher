using System.Windows;
using System.Windows.Controls;

namespace Fluor.ProjectSwitcher
{
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProjectTemplate { get; set; }
        public DataTemplate ApplicationTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate MenuItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            MenuItem menuItem = (MenuItem)item;
            //MenuItem menuItem = item as MenuItem;

            if (menuItem != null)
            {
                return MenuItemTemplate;
            }
            else
            {
                return SeparatorTemplate;
            }

            //SPPIDApp sppidApp = item as SPPIDApp;

            //if (sppidApp != null)
            //{
            //    if (sppidApp.ParentApp == "SEP")
            //    {
            //        return SeparatorTemplate;
            //    }
            //    else if (sppidApp.ParentApp == "HEADER")
            //    {
            //        return HeaderTemplate;
            //    }
            //    return ApplicationTemplate;
            //}
        }
    }
}
