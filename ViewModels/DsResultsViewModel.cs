using HRM_Viewer.Models;
using HRM_Viewer.Services;
using HRM_Viewer.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HRM_Viewer.ViewModels
{
    public class DsResultsViewModel : ObservableObject
    {
        private DataAccessService _dsService;
        private AppException _appEx;
        private Dictionary<string, string> _sqlCommands;

        private SettingObject _fullDSName;
        private DataTable _dsRecordSet;
        private ParsedHtmlsCollection _pHtmlCollection;

        private string _mainHtmlID;

        private Dictionary<string, List<ValidationRule>> _dsNameValidationRules;

        public DataTable DSRecordSet
        {
            get => _dsRecordSet;
            private set
            {
                _dsRecordSet = value;
                OnPropChanged(nameof(this.DSRecordSet));
            }
        }

        public SettingObject FullDSName
        {
            private set
            {
                _fullDSName = value;
                OnPropChanged(nameof(FullDSName));
            }
            get { return _fullDSName; }
        }

        public string CurrentHtmlCode
        {
            private set
            {
                _pHtmlCollection.CurrentHtml.HtmlCode = value;
                OnPropChanged(nameof(this.CurrentHtmlCode));
            }
            get { return _pHtmlCollection.CurrentHtml.ToString(); }
        }

        public string CurrentHtmlNo
        {
            private set
            {
                _pHtmlCollection.CurrentHtmlNo = int.Parse(value);
                OnPropChanged(nameof(this.CurrentHtmlNo));
            }
            get
            {
                if (_pHtmlCollection.NoOfHtmls == 0)
                    return "0";

                return (_pHtmlCollection.CurrentHtmlNo + 1).ToString();
            }
        }

        public string NoOfHtmls
        {
            private set
            {
                _pHtmlCollection.NoOfHtmls = int.Parse(value);
                OnPropChanged(nameof(this.NoOfHtmls));
            }
            get { return _pHtmlCollection.NoOfHtmls.ToString(); }
        }

        public ParsedHtmlsCollection PHtmlCollection
        {
            get => _pHtmlCollection;
            set
            {
                _pHtmlCollection = value;
                OnPropChanged(nameof(PHtmlCollection));
            }
        }

        public RelayCommand GetDSRecordsCmd { get; private set; }
        public RelayCommand GetParsedHtmlsCmd { get; private set; }
        public RelayCommand ChangeHtmlCmd { get; private set; }

        public DsResultsViewModel(ref AppException appEx, ref DataAccessService dsService, Dictionary<string, string> sqlCommands)
        {
            if (appEx == null || dsService == null || sqlCommands == null)
                throw new NullReferenceException("appEx/dsService/sqlCommands");

            _dsService = dsService;
            _appEx = appEx;
            _sqlCommands = sqlCommands;

            _mainHtmlID = string.Empty;

            _dsRecordSet = new DataTable();
            _pHtmlCollection = new ParsedHtmlsCollection();

            _dsNameValidationRules = new Dictionary<string, List<ValidationRule>>();
            _dsNameValidationRules.Add(nameof(SettingObject.SettingValue), new List<ValidationRule>() { new FullDSNameRule() });
            _fullDSName = new SettingObject("Datasource", string.Empty, _dsNameValidationRules);

            GetDSRecordsCmd = new RelayCommand(ref appEx, GetDSRecords, GetDSRecordsCanUse);
            GetParsedHtmlsCmd = new RelayCommand(ref appEx, GetParsedHtmls, GetParsedHtmlsCanUse);
            ChangeHtmlCmd = new RelayCommand(ref appEx, ChangeHtml, ChangeHtmlCanUse);
        }

        public void ChangeHtml(object o)
        {
            _pHtmlCollection.ChangeHtml(o.ToString());
            UpdateView();
        }
        public bool ChangeHtmlCanUse(object o)
        {
            try
            {
                if (int.Parse(NoOfHtmls) <= 0)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void GetDSRecords(object o)
        {
            var dsRecordSet = new DataTable();
            string dsName = this._fullDSName.SettingValue;

            var parameters = new Dictionary<string, string>();
            parameters.Add("@dsName", dsName);
            dsRecordSet = _dsService.ReturnDSRecords(_sqlCommands["SelectDSRecords"], parameters);

            this.DSRecordSet = dsRecordSet;
        }

        public bool GetDSRecordsCanUse(object o)
        {
            if (this.FullDSName.HasErrors)
                return false;

            return true;
        }

        public void GetParsedHtmls(object o)
        {
            var dsRecordSet = new DataTable();
            
            var parameters = new Dictionary<string, string>();
            parameters.Add("@ID", this._mainHtmlID);
            parameters.Add("@outM", "S");
            dsRecordSet = _dsService.ReturnDSRecords(_sqlCommands["ParsedHtmls"], parameters);

            _pHtmlCollection.ParseHtmlData(dsRecordSet);

            UpdateView();
        }

        public bool GetParsedHtmlsCanUse(object o)
        {
            if (string.IsNullOrEmpty(_mainHtmlID) || string.IsNullOrWhiteSpace(_mainHtmlID))
                return false;

            try
            {
                int tmp = int.Parse(_mainHtmlID);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void GetFullDSname(string reqHtmlId, DataTable templateInfo)
        {
            this.FullDSName.SettingValue = string.Empty;
            this.DSRecordSet.Clear();
            foreach(DataRow r in templateInfo.Rows)
            {
                if (r["htmlID"].ToString() == reqHtmlId)
                {
                    this.FullDSName.SettingValue = r["fullDatasourceName"].ToString();
                    break;
                }
            }
        }

        public void SetCurrMainHtmlId(string id)
        {
            _mainHtmlID = id;
        }

        public void UpdateView()
        {
            this.CurrentHtmlCode = _pHtmlCollection.CurrentHtml.HtmlCode;
            this.CurrentHtmlNo = _pHtmlCollection.CurrentHtmlNo.ToString();
            this.NoOfHtmls = _pHtmlCollection.NoOfHtmls.ToString();
        }

        public void SaveParsedHtml(string basePath)
        {
            int i = 0;
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            //str = rgx.Replace(str, "");

            string writeableName;
            string path;

            foreach (ParsedHtml h in _pHtmlCollection.ParsedHtmls)
            {
                writeableName = rgx.Replace(h.MailSubject, "") + $"_{i}";
                path = basePath + "/" + writeableName + ".htm";

                if (File.Exists(path))
                    File.Delete(path);

                // Create the file, or overwrite if the file exists.
                File.WriteAllText(path, h.ToString());

                i += 1;
            }
        }

    }
}
