using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluor.ProjectSwitcher.Message
{
    public class M_SimpleAction
    {
        public enum Action
        {
            DisplayApplicationsAsTiles,
            RefreshViews
        }

        public Action SimpleAction;

        public M_SimpleAction(Action simpleAction)
        {
            SimpleAction = simpleAction;
        }
    }
}
