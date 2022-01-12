using HRM_Viewer.Models;
using HRM_Viewer.Services;
using HRM_Viewer.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;

namespace HRM_Viewer.ViewModels
{
    public class SettingsViewModel
    {
        private DataAccessService _dsService; 
        
        private List<SettingObject> _mainSettings;
        private List<SettingObject> _mValidatedSettings;

        private readonly Dictionary<string, List<ValidationRule>> _mainSettingValRules;
        private readonly Dictionary<string, List<ValidationRule>> _optionalSettingValRules;
        private readonly Dictionary<string, List<ValidationRule>> _psswdRules;

        private Dictionary<string, string> _sqlCommands;

        public RelayCommand ApplySettingsCmd { get; private set; }
        public RelayCommand RefreshSettingsCmd { get; private set; }
        public RelayCommand TestSQLConnectionCmd { get; private set; }

        public List<SettingObject> MainSettings
        {
            set{ _mainSettings = value; }
            get{ return _mainSettings; }
        }

        public List<SettingObject> MValidatedSettings
        {
            set
            { _mValidatedSettings = value; }
            get
            { return _mValidatedSettings; }
        }

        public string SQLConnPswd
        {
            set
            {
                foreach (SettingObject s in this.MainSettings)
                {
                    if (s.SettingName.ToLower() == "password")
                        s.SettingValue = value;
                }
            }
        }

        public SettingsViewModel(ref AppException appEx, ref DataAccessService dsService, Dictionary<string, string> sqlCommands)
        {
            if (appEx == null || dsService == null || sqlCommands == null)
                throw new NullReferenceException("appEx/dsService/sqlCommands");

            this._dsService = dsService;

            _mainSettingValRules = new Dictionary<string, List<ValidationRule>>();
            _mainSettingValRules.Add(nameof(PasswordObject.SettingValue), new List<ValidationRule>() { new MainSettingRule() });
            _optionalSettingValRules = new Dictionary<string, List<ValidationRule>>();
            _optionalSettingValRules.Add(nameof(PasswordObject.SettingValue), new List<ValidationRule>() { new MainOptionalSettingRule() });
            _psswdRules = new Dictionary<string, List<ValidationRule>>();
            _psswdRules.Add(nameof(PasswordObject.SettingValue), new List<ValidationRule>() { new NotEmptyPasswordRule() });

            MainSettings = new List<SettingObject>()
            {
                new SettingObject("SQL Server Machine", string.Empty, _mainSettingValRules),
                new SettingObject("SQL Server Instance", string.Empty, _optionalSettingValRules),
                new SettingObject("Username", string.Empty, _mainSettingValRules),
                new PasswordObject("Password", string.Empty, new Dictionary<string, List<ValidationRule>>()),
                new SettingObject("DatabaseName", string.Empty, _mainSettingValRules)
            };

            MValidatedSettings = new List<SettingObject>()
            {
                new SettingObject("SQL Server Machine", string.Empty, _mainSettingValRules),
                new SettingObject("SQL Server Instance", string.Empty, _optionalSettingValRules),
                new SettingObject("Username", string.Empty, _mainSettingValRules),
                new PasswordObject("Password", string.Empty, new Dictionary<string, List<ValidationRule>>()),
                new SettingObject("DatabaseName", string.Empty, _mainSettingValRules)
            };

            ApplySettingsCmd = new RelayCommand(ref appEx, ApplySettings, ApplySettingsCanUse);
            RefreshSettingsCmd = new RelayCommand(ref appEx, RefreshSettings);
            TestSQLConnectionCmd = new RelayCommand(ref appEx, TestSQLConnection, TestSQLConnectionCanUse);

            _sqlCommands = sqlCommands;

            ReadSettings();
        }


        public void ApplySettings(object o)
        {
            foreach (SettingObject s1 in this.MValidatedSettings)
            {
                foreach (SettingObject s2 in this.MainSettings)
                {
                    if (s1.SettingName.ToLower() == s2.SettingName.ToLower())
                        s1.SettingValue = s2.SettingValue;
                }
            }

            _dsService.ParseConnectionString(this.MValidatedSettings);
            _dsService.GetAvailableTemplates(_sqlCommands["AvailableTemplates"], new Dictionary<string, string>());
        }

        public bool ApplySettingsCanUse(object o)
        {
            foreach (SettingObject vo in this.MainSettings)
            {
                if (vo.HasErrors == true)
                    return false;
            }

            return true;
        }

        public void RefreshSettings(object o)
        {
            foreach (SettingObject s1 in this.MainSettings)
            {
                foreach (SettingObject s2 in this.MValidatedSettings)
                {
                    if (s1.SettingName.ToLower() == s2.SettingName.ToLower())
                        s1.SettingValue = s2.SettingValue;
                }
            }
        }

        public void TestSQLConnection(object o)
        {
            _dsService.TestConnection();
        }

        public bool TestSQLConnectionCanUse(object o)
        {
            foreach (SettingObject vo in this.MValidatedSettings)
            {
                if (vo.HasErrors == true)
                    return false;
            }

            return true;
        }

        public void ReadSettings()
        {
            XmlDocument settingsDoc = new XmlDocument();
            string xmlPath = Directory.GetCurrentDirectory() + @"\settings.xml";

            try
            {
                settingsDoc.Load(xmlPath);

                XmlNodeList settingsList = settingsDoc.GetElementsByTagName("settings");

                foreach (XmlNode parentSetting in settingsList)
                {
                    foreach (XmlNode setting in parentSetting.ChildNodes)
                    {
                        foreach (SettingObject s in this._mainSettings)
                        {
                            if (s.SettingName.ToLower().Replace(" ", "") == setting.Name.ToLower())
                                s.SettingValue = setting.InnerText;
                        }

                        foreach (SettingObject s in this._mValidatedSettings)
                        {
                            if (s.SettingName.ToLower().Replace(" ", "") == setting.Name.ToLower())
                                s.SettingValue = setting.InnerText;
                        }
                    }
                }
            }
            catch
            {}
        }

        public void SaveSettings()
        {
            string xmlPath = Directory.GetCurrentDirectory() + @"\settings.xml";

            if (File.Exists(xmlPath))
            {
                File.Delete(xmlPath);
            }

           
            using (XmlWriter writer = XmlWriter.Create(xmlPath))
            {
                writer.WriteStartElement("settings");
                
                foreach (SettingObject s in this.MValidatedSettings.Where(o => o.SettingName.ToLower() != "password"))
                {
                    writer.WriteElementString(s.SettingName.Replace(" ", ""), s.SettingValue);
                }

                writer.WriteEndElement();
                writer.Flush();
            }
        }
    }
}
