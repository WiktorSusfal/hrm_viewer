using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using HRM_Viewer.Utilities;

namespace HRM_Viewer.Models
{
    public class SettingObject : ValidatableObject
    { 
        private string _settingName;
        private string _settingValue;

        public SettingObject()
        {
            this.errors = new Dictionary<string, List<string>>();
            this.validationRules = new Dictionary<string, List<ValidationRule>>();

            this.SettingName = string.Empty;
            this.SettingValue = string.Empty;
        }

        public SettingObject(string sName, string defaultValue, Dictionary<string, List<ValidationRule>> validationRules)
        {
            this.errors = new Dictionary<string, List<string>>();
            this.validationRules = new Dictionary<string, List<ValidationRule>>();
            this.validationRules = validationRules;

            this.SettingName = sName;
            this.SettingValue = defaultValue;
        }

        public string SettingName
        {
            get { return _settingName; }
            set
            {
                if (value != null)
                {
                    _settingName = value.ToString();
                    ValidateProperty(value);
                }

                OnPropChanged(nameof(SettingName));
            }
        }

        public string SettingValue
        {
            get { return _settingValue; }
            set
            {
                if (value != null)
                {
                    _settingValue = value.ToString();
                    ValidateProperty(value);
                }
                OnPropChanged(nameof(SettingValue));

            }
        }

    }
}
