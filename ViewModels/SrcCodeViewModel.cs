using HRM_Viewer.Models;
using HRM_Viewer.Services;
using HRM_Viewer.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Viewer.ViewModels
{
    public class SrcCodeViewModel : ObservableObject
    {

        private HtmlCodeTemplate _codeTemplate;

        public RelayCommand HighlightParametersCmd { get; private set; }

        public List<string> DisplayedParameters
        {
            set
            {
                _codeTemplate.HtmlParameters = value;
                OnPropChanged("DisplayedParameters");
            }
            get
            {
                return _codeTemplate.HtmlParameters.Distinct().ToList();
            }
        }

        public string HtmlCode
        {
            get => _codeTemplate.HtmlCode;
            set
            {
                _codeTemplate.HtmlCode = value;
                OnPropChanged(nameof(this.HtmlCode));
            }
        }

        public string OriginalHtmlCode
        {
            get => _codeTemplate.OriginalHtmlCode;
            private set
            {
                _codeTemplate.OriginalHtmlCode = value;
                OnPropChanged(nameof(this.OriginalHtmlCode));
            }
        }

        public SrcCodeViewModel(ref AppException appEx)
        {
            _codeTemplate = new HtmlCodeTemplate(string.Empty, string.Empty, string.Empty, new List<string>(), new List<string>(), new List<string>());

            HighlightParametersCmd = new RelayCommand(ref appEx, HighlightParameters, HighlightParametersCanUse);
        }

        public void ParseHtmlTemplateCodeInfo(string reqHtmlId, DataTable templateInfoSet, DataTable templateParamsSet)
        {
            _codeTemplate.ParseHtmlTemplateCodeInfo(reqHtmlId, templateInfoSet, templateParamsSet);
            this.DisplayedParameters = _codeTemplate.MainHtmlParameters.Concat(_codeTemplate.PrefixHtmlParameters).Concat(_codeTemplate.SuffixHtmlParameters).ToList();
            this.HtmlCode = _codeTemplate.PrefixOrgHtmlCode + _codeTemplate.MainOrgHtmlCode + _codeTemplate.SuffixOrgHtmlCode;
            this.OriginalHtmlCode = _codeTemplate.OriginalHtmlCode;
        }

        public void HighlightParameters(object o)
        {
            string resultCode = _codeTemplate.OriginalHtmlCode;
            string fullParamSyntax = string.Empty;
            string highlightedValue = string.Empty;

            System.Collections.IList items = (System.Collections.IList)o;
            var paramList = items.Cast<string>();

            foreach (string paramName in paramList)
            {
                fullParamSyntax = _codeTemplate.ParamPrefix + paramName + _codeTemplate.ParamSuffix;
                highlightedValue = "<span style=\"background - color:#FFA500;\"><strong>" + fullParamSyntax + "</strong></span>";

                resultCode = resultCode.Replace(fullParamSyntax, highlightedValue);
            }

            this.HtmlCode = resultCode;
        }

        public bool HighlightParametersCanUse(object o)
        {
            if (this.DisplayedParameters.Count() <= 0)
                return false;

            return true;
        }
    }
}
