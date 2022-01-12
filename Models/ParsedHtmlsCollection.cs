using HRM_Viewer.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Viewer.Models
{
    public class ParsedHtmlsCollection : ObservableObject
    {
        private ParsedHtml _currentHtml;
        private List<ParsedHtml> _parsedHtmls;

        private int _currentHtmlNo;
        private int _noOfHtmls;

        public ParsedHtmlsCollection()
        {
            _parsedHtmls = new List<ParsedHtml>();

            _currentHtmlNo = 0;
            _noOfHtmls = _parsedHtmls.Count;
            _currentHtml = _parsedHtmls.FirstOrDefault();
        }

        public ParsedHtml CurrentHtml
        {
            get
            {
                if (_currentHtml == null)
                    return new ParsedHtml("&nbsp;", string.Empty, string.Empty);

                return _currentHtml;
            }
            set
            {
                _currentHtml = value;
                OnPropChanged(nameof(this.CurrentHtml));
            }
        }

        public int NoOfHtmls
        {
            get => _noOfHtmls;
            set
            {
                _noOfHtmls = value;
                OnPropChanged(nameof(this.NoOfHtmls));
            }
        }

        public int CurrentHtmlNo
        {
            get => _currentHtmlNo;
            set
            {
                _currentHtmlNo = value;
                OnPropChanged(nameof(this.CurrentHtmlNo));
            }
        }

        public List<ParsedHtml> ParsedHtmls
        {
            get => _parsedHtmls;
        }

        public void GoFirst()
        {
            CurrentHtmlNo = 0;
            CurrentHtml = _parsedHtmls.FirstOrDefault();
        }

        public void GoNext()
        {
            if (CurrentHtmlNo < (NoOfHtmls - 1))
            {
                CurrentHtmlNo += 1;
                CurrentHtml = _parsedHtmls[CurrentHtmlNo];
            }
        }

        public void GoPrevious()
        {
            if (CurrentHtmlNo > 0)
            {
                CurrentHtmlNo -= 1;
                CurrentHtml = _parsedHtmls[CurrentHtmlNo];
            }
        }

        public void GoLast()
        {
            CurrentHtmlNo = NoOfHtmls -1;
            CurrentHtml = _parsedHtmls[CurrentHtmlNo];
        }

        public void ChangeHtml(string param)
        {
            if (param == "-2")
                GoFirst();
            else if (param == "-1")
                GoPrevious();
            else if (param == "1")
                GoNext();
            else if (param == "2")
                GoLast();
        }

        public void ParseHtmlData(DataTable t)
        {
            var tmpList = new List<ParsedHtml>();

            foreach (DataRow r in t.Rows)
            {
                tmpList.Add( new ParsedHtml(r["html"].ToString(), r["mAddress"].ToString(), r["mSubject"].ToString()) );
            }

            _parsedHtmls = tmpList;
            this.CurrentHtml = _parsedHtmls.FirstOrDefault();
            this.CurrentHtmlNo = 0;
            this.NoOfHtmls = _parsedHtmls.Count();
        }
    }

    public class ParsedHtml
    {
        public string HtmlCode { get; set; }
        public string MailAddress { get; set; }
        public string MailSubject { get; set; }

        public ParsedHtml(string htmlCode = "&nbsp;", string mailAddress = "", string mailSubject = "")
        {
            this.HtmlCode = htmlCode;
            this.MailAddress = mailAddress;
            this.MailSubject = mailSubject;
        }

        public override string ToString()
        {
            string code = string.Empty;

            code += "<p><strong>Mail Address:</strong>&emsp;" + MailAddress + "<br>";
            code += "<strong>Subject:</strong>&emsp;" + MailSubject + "</p>";
            code += HtmlCode;

            return code;
        }
    }
}
