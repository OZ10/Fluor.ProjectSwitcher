using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluor.ProjectSwitcher.Class
{
    public class Parameter
    {
        public enum TypeEnum
        {
            INI,
            REG,
            PATH
        };

        public TypeEnum Type { get; set; }
        public string Value { get; set; }
        public string Path { get; set; }

        public Parameter(TypeEnum type, string value, string path)
        {
            Type = type;
            Value = value;
            Path = ValidatePath(path);
        }

        private string ValidatePath(string path)
        {
            if (path.Contains("%"))
            {
                string usersProfilePath = Environment.ExpandEnvironmentVariables("%userprofile%");
                path = path.Replace("%userprofile%", "");
                path = usersProfilePath + path;
            }
            else if (path.Contains("*"))
            {
                //TODO install paths that contain the variable *installpath* cannot be populated here because
                //associations are evaluated BEFORE the application installation path. Need to move somewhere else.
                //string installPath = Environment.ExpandEnvironmentVariables("%userprofile%");
                //path = path.Replace("%installpath%", "");
                //path = installPath + path;
            }
            return path;
        }

        public List<string> GetSettings()
        {
            return Value.Split(new string[] { "," }, StringSplitOptions.None).ToList();
        }

        public string GetInstallationPath()
        {
            switch (Type)
            {
                case TypeEnum.INI:
                    break;
                case TypeEnum.REG:
                    string[] reg = Path.Split(',');
                    Path = (string)Registry.GetValue(@reg[0], reg[1], null);

                    if (Path != null)
                    {
                        if (Value != null)
                        {
                            return Path.Insert(Path.Length, Value);
                        }
                    }
                    return Path;
                case TypeEnum.PATH:
                    return Path;
                default:
                    break;
            }
            return "";
        }
    }
}
