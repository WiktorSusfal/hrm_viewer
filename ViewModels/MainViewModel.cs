using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_Viewer.Utilities;
using HRM_Viewer.Models;
using System.Windows.Controls;
using HRM_Viewer.Services;
using System.Windows.Input;
using System.Resources;
using System.Reflection;

namespace HRM_Viewer.ViewModels
{
    public class MainViewModel
    {
        private SettingsViewModel _mStModel;
        private ViewerViewModel _vModel;
        private AppException _appEx;
        private DataAccessService _dsService;
        private WindowDialogService _wDService;

        private Dictionary<string, string> _sqlCommands;
        
        public SettingsViewModel MStModel
        {
            private set { _mStModel = value; }
            get { return _mStModel; }
        }

        public ViewerViewModel VModel
        {
            private set { _vModel = value; }
            get { return _vModel; }
        }

        public AppException AppEx
        {
            get => _appEx;
            private set { _appEx = value; }
        }

        public RelayCommand SaveSettingsCmd { get; private set; }
        public RelayCommand ReadSettingsCmd { get; private set; }
        public RelayCommand SaveParsedHtmlsCmd { get; private set; }

        public MainViewModel()
       {
            _dsService = new DataAccessService("MSSQL_2014");
            _appEx = new AppException();
            _wDService = new WindowDialogService();

            _sqlCommands = new Dictionary<string, string>();
            _sqlCommands.Add("HtmlTemplateInfo", "dbo.HRM_04_ReturnHTemplateInfo @id");
            _sqlCommands.Add("HtmlTemplateParams", "dbo.HRM_03_ReturnParamsList @reportId");
            _sqlCommands.Add("ParsedHtmls", "dbo.HRM_00_PrepareHtmls @htmlID=@ID, @outputMode='@outM'");
            _sqlCommands.Add("SelectDSRecords", "SELECT * FROM @dsName");
            _sqlCommands.Add("AvailableTemplates", "SELECT ID, HName from dbo.Report_HTMLS");

            this._mStModel = new SettingsViewModel(ref _appEx, ref _dsService, _sqlCommands);
            this._vModel = new ViewerViewModel(ref _appEx, ref _dsService, _sqlCommands);

            SaveSettingsCmd = new RelayCommand(ref _appEx, SaveSettings, SaveSettingsCanUse);
            ReadSettingsCmd = new RelayCommand(ref _appEx, ReadSettings);
            SaveParsedHtmlsCmd = new RelayCommand(ref _appEx, SaveParsedHtmls, SaveParsedHtmlsCanUse);
        }

        public void SaveSettings(object o)
        {
            _mStModel.SaveSettings();
        }

        public bool SaveSettingsCanUse(object o)
        {
            foreach (SettingObject s in _mStModel.MValidatedSettings)
            {
                if (s.HasErrors)
                    return false;
            }

            return true;
        }

        public void ReadSettings(object o)
        {
            _mStModel.ReadSettings();
        }

        public void SaveParsedHtmls(object o)
        {
            string basePath;

            basePath = _wDService.SelectFolderPath();

            if (!string.IsNullOrEmpty(basePath))
                _vModel.DRVM.SaveParsedHtml(basePath);
        }

        public bool SaveParsedHtmlsCanUse(object o)
        {
            try
            {
                if (int.Parse(_vModel.DRVM.NoOfHtmls) <= 0 || _vModel.DRVM.NoOfHtmls == null)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
