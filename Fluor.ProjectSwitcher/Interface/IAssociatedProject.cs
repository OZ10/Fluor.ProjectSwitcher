using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluor.ProjectSwitcher.Interface
{
    public interface IAssociatedProject
    {
        string Name { get; set; }
        string ID { get; set; }
    }
}
