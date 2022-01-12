using HRM_Viewer.Models;
using HRM_Viewer.Services;
using HRM_Viewer.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HRM_Viewer.ViewModels
{
    public class ViewerViewModel : ObservableObject
    {
        private DataAccessService _dsService;

        private SrcCodeViewModel _sCVM;
        private DsResultsViewModel _dRVM;
        private DataTable _htmlTemplateInfo;

        private Dictionary<string, string> _sqlCommands;
        private Dictionary<string, List<ValidationRule>> _numberGTThanZeroRules;

        public DataTable HtmlTemplateInfo
        {
            get { return _htmlTemplateInfo; }
            private set
            {
                _htmlTemplateInfo = value;
                OnPropChanged(nameof(this.HtmlTemplateInfo));
            }
        }

        public SrcCodeViewModel SCVM
        {
            get => _sCVM;
            private set { _sCVM = value; }
        }

        public DsResultsViewModel DRVM
        {
            get => _dRVM;
            private set { _dRVM = value; }
        }

        public DataAccessService DSService
        {
            get => _dsService;
        }

        public RelayCommand GetHtmlTemplateInfoCmd { get; private set; }

        public ViewerViewModel(ref AppException appEx, ref DataAccessService dsService, Dictionary<string, string> sqlCommands)
        {
            if (appEx == null || dsService == null || sqlCommands == null)
                throw new NullReferenceException("appEx/dsService/sqlCommands");

            _dsService = dsService;
            _sqlCommands = sqlCommands;

            _htmlTemplateInfo = new DataTable();
            _sCVM = new SrcCodeViewModel(ref appEx);
            _dRVM = new DsResultsViewModel(ref appEx, ref dsService, sqlCommands);

            _numberGTThanZeroRules = new Dictionary<string, List<ValidationRule>>();
            _numberGTThanZeroRules.Add(nameof(SettingObject.SettingValue), new List<ValidationRule>() { new MustBeNumberGTZeroRule() });

            GetHtmlTemplateInfoCmd = new RelayCommand(ref appEx, GetHtmlTemplateInfo, GetHtmlTemplateInfoCanUse);
        }

        public void GetHtmlTemplateInfo(object o)
        {
            var templateResultSet = new DataTable();
            var htmlParametersSet = new DataTable();

            string reqId = DSService.SelectedTemplate;

            var parameters = new Dictionary<string, string>();
            parameters.Add("@id", reqId);
            templateResultSet = _dsService.ReturnDSRecords(_sqlCommands["HtmlTemplateInfo"], parameters);
            this.HtmlTemplateInfo = templateResultSet;

            parameters = new Dictionary<string, string>();
            parameters.Add("@reportId", reqId);
            htmlParametersSet = _dsService.ReturnDSRecords(_sqlCommands["HtmlTemplateParams"], parameters);

            _sCVM.ParseHtmlTemplateCodeInfo(reqId, templateResultSet, htmlParametersSet);

            _dRVM.GetFullDSname(reqId, templateResultSet);
            _dRVM.SetCurrMainHtmlId(reqId);
            _dRVM.PHtmlCollection = new ParsedHtmlsCollection();
            _dRVM.UpdateView();
        }

        public bool GetHtmlTemplateInfoCanUse(object o)
        {
            if (string.IsNullOrEmpty(this.DSService.SelectedTemplate))
                return false;

            try
            {
                int i = int.Parse(this.DSService.SelectedTemplate);
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}
