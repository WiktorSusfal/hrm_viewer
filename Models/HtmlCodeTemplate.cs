using HRM_Viewer.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Viewer.Models
{
    public class HtmlCodeTemplate : ObservableObject
    {
 
        private string _mainOriginalHtmlCode;
        private string _prefixOriginalHtmlCode;
        private string _suffixOriginalHtmlCode;
        private string _originalHtmlCode;
        private string _htmlCode;
        private string _paramPrefix;
        private string _paramSuffix;
        private List<string> _mainHtmlParameters;
        private List<string> _prefixHtmlParameters;
        private List<string> _suffixHtmlParameters;
        private List<string> _htmlParameters;

        public HtmlCodeTemplate(string mainOriginalCode, string prefixOriginalCode, string suffixOriginalCode, List<string> mainHtmlParameters, List<string> prefixHtmlParameters, List<string> suffixHtmlParameters)
        {

            this._mainOriginalHtmlCode = mainOriginalCode;
            this._prefixOriginalHtmlCode = prefixOriginalCode;
            this._suffixOriginalHtmlCode = suffixOriginalCode;

            this._mainHtmlParameters = mainHtmlParameters;
            this._suffixHtmlParameters = suffixHtmlParameters;
            this._prefixHtmlParameters = prefixHtmlParameters;
            this._originalHtmlCode = prefixOriginalCode + mainOriginalCode + suffixOriginalCode;

            this.HtmlCode = string.Empty;
            this.HtmlParameters = new List<String>();

        }

        public string MainOrgHtmlCode
        {
            get => _mainOriginalHtmlCode;
            set
            {
                _mainOriginalHtmlCode = value;
                OnPropChanged(nameof(this.MainOrgHtmlCode));
            }
        }
        public string PrefixOrgHtmlCode
        {
            get => _prefixOriginalHtmlCode;
            set
            {
                _prefixOriginalHtmlCode = value;
                OnPropChanged(nameof(this.PrefixOrgHtmlCode));
            }
        }

        public string SuffixOrgHtmlCode
        {
            get => _suffixOriginalHtmlCode;
            set
            {
                _suffixOriginalHtmlCode = value;
                OnPropChanged(nameof(this.SuffixOrgHtmlCode));
            }
        }

        public string HtmlCode
        {
            get => _htmlCode;
            set
            {
                _htmlCode = value;
                OnPropChanged(nameof(this.HtmlCode));
            }
        }

        public string OriginalHtmlCode
        {
            get => _originalHtmlCode;
            set
            {
                _originalHtmlCode = value;
                OnPropChanged(nameof(this.OriginalHtmlCode));
            }
        }

        public string ParamPrefix
        {
            get => _paramPrefix;
        }

        public string ParamSuffix
        {
            get => _paramSuffix;
        }

        public List<string> MainHtmlParameters
        {
            get => _mainHtmlParameters;
            set
            {
                _mainHtmlParameters = value;
                OnPropChanged(nameof(this.MainHtmlParameters));
            }
        }

        public List<string> SuffixHtmlParameters
        {
            get => _suffixHtmlParameters;
            set
            {
                _suffixHtmlParameters = value;
                OnPropChanged(nameof(this.SuffixHtmlParameters));
            }
        }

        public List<string> PrefixHtmlParameters
        {
            get => _prefixHtmlParameters;
            set
            {
                _prefixHtmlParameters = value;
                OnPropChanged(nameof(this.PrefixHtmlParameters));
            }
        }

        public List<string> HtmlParameters
        {
            get => _htmlParameters;
            set
            {
                _htmlParameters = value;
                OnPropChanged(nameof(this.HtmlParameters));
            }
        }

        public void ParseHtmlTemplateCodeInfo(string reqId, DataTable templateInfo, DataTable htmlParameters)
        {
            string mainCode = string.Empty;
            string prefixCode = string.Empty;
            string suffixCode = string.Empty;

            string paramPrefix = string.Empty;
            string paramSuffix = string.Empty;

            string prefixId = string.Empty;
            string suffixId = string.Empty;

            var mainParams = new List<string>();
            var prefixParams = new List<string>();
            var suffixParams = new List<string>();

            mainParams = (from DataRow r in htmlParameters.Rows
                          where r["ReportID"].ToString() == reqId
                          select r["ParamName"].ToString()).ToList();

            foreach (DataRow r in templateInfo.Rows)
            {
                if (r["htmlID"].ToString() == reqId)
                {
                    mainCode = r["htmlCode"].ToString();
                    prefixId = r["preffixHtmlId"].ToString();
                    suffixId = r["suffixHtmlId"].ToString();

                    paramPrefix = r["sParamPattern"].ToString();
                    paramSuffix = r["eParamPattern"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(prefixId))
            {
                prefixParams = (from DataRow r in htmlParameters.Rows
                                where r["ReportID"].ToString() == prefixId
                                select r["ParamName"].ToString()).ToList();

                foreach (DataRow r in templateInfo.Rows)
                {
                    if (r["htmlID"].ToString() == prefixId)
                        prefixCode = r["htmlCode"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(suffixId))
            {
                suffixParams = (from DataRow r in htmlParameters.Rows
                                where r["ReportID"].ToString() == suffixId
                                select r["ParamName"].ToString()).ToList();

                foreach (DataRow r in templateInfo.Rows)
                {
                    if (r["htmlID"].ToString() == suffixId)
                        suffixCode = r["htmlCode"].ToString();
                }
            }

            MainOrgHtmlCode = mainCode;
            PrefixOrgHtmlCode = prefixCode;
            SuffixOrgHtmlCode = suffixCode;
            _originalHtmlCode = prefixCode + mainCode + suffixCode;
            _paramPrefix = paramPrefix;
            _paramSuffix = paramSuffix;
            _prefixHtmlParameters = prefixParams;
            _suffixHtmlParameters = suffixParams;
            _mainHtmlParameters = mainParams;
        }
    }
}
