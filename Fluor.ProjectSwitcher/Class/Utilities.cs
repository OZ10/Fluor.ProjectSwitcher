using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluor.ProjectSwitcher.Class
{
    static public class Utilities
    {
        static public void OpenFolder(string folderPath)
        {
            // Open windows explorer
            Process p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = folderPath;
            p.Start();
        }
    }
}
