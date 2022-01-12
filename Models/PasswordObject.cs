using HRM_Viewer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HRM_Viewer.Models
{
    public class PasswordObject : SettingObject
    {
        public PasswordObject(string sName, string defaultValue, Dictionary<string, List<ValidationRule>> validationRules) : base(sName, defaultValue, validationRules)
        {

        }
    }
}
