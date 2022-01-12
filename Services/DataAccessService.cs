using HRM_Viewer.Models;
using HRM_Viewer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Viewer.Services
{
    public class DataAccessService : ObservableObject, ISQLDataAccess
    {

        private string _connectionString;
        private string _connStrID;

        private ObservableCollection<KeyValuePair<string, string>> _availableTamplates;
        private string _selectedTemplate;

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public ObservableCollection<KeyValuePair<string, string>> AvailableTemplates
        {
            get => _availableTamplates;
        }

        public string SelectedTemplate
        {
            set
            {
                string templateInfo = (string)value;
                if (templateInfo == null)
                {
                    _selectedTemplate = string.Empty;
                    return;
                }

                int index = templateInfo.IndexOf(" - ");

                if (index <= 0)
                {
                    _selectedTemplate = string.Empty;
                    return;
                }

                templateInfo = templateInfo.Substring(0, index);
                templateInfo.Replace(" ", "");

                _selectedTemplate = templateInfo;

            }
            get => _selectedTemplate;
        }


        public DataAccessService(string connStrID)
        {
            _connectionString = string.Empty;

            if (connStrID == null)
                throw new NullReferenceException("connStrID");

            _connStrID = connStrID;

            _availableTamplates = new ObservableCollection<KeyValuePair<string, string>>();
            _selectedTemplate = string.Empty;
        }

        public DataTable ReturnDSRecords(string query, Dictionary<string, string> parameters)
        {
            query = ParseQuery(query, parameters);
            var resultSet = new DataTable();

            using (SqlConnection sqlConn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand sCmd = new SqlCommand(query, sqlConn);
                SqlDataAdapter sqlDA = new SqlDataAdapter(sCmd);
                sqlDA.Fill(resultSet);

                sqlConn.Close();
            }

            return resultSet;
        }

        public DataTable ReturnDSSchema(string query, Dictionary<string, string> parameters)
        {
            query = ParseQuery(query, parameters);
            DataTable schemaTable = new DataTable();

            using (SqlConnection sqlConn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand sCmd = new SqlCommand(query, sqlConn);
                sqlConn.Open();

                using (SqlDataReader sqlReader = sCmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    schemaTable = sqlReader.GetSchemaTable();
                }

                sqlConn.Close();
            }

            return schemaTable;
        }

        public void TestConnection()
        {
            SqlConnection sqlConn = new SqlConnection(this.ConnectionString);

            sqlConn.Open();
            sqlConn.Close();
            sqlConn.Dispose();
        }

        public void ParseConnectionString(List<SettingObject> s)
        {
            string connectionStr = ConfigurationManager.ConnectionStrings[_connStrID].ConnectionString;

            foreach (SettingObject v in s)
            {
                connectionStr = connectionStr.Replace("{" + v.SettingName + "}", v.SettingValue);
            }

            // In case Sql Instance Name is empty, remove the last single slash from <machinename>/<instancename> property. 
            connectionStr = connectionStr.Replace("/;", ";");
            this.ConnectionString = connectionStr;
        }

        public string ParseQuery(string query, Dictionary<string, string> parameters)
        {
            string paramValue;

            foreach(string key in parameters.Keys)
            {
                parameters.TryGetValue(key, out paramValue);
                query = query.Replace(key, paramValue);
            }

            return query;
        }

        public void GetAvailableTemplates(string query, Dictionary<string, string> parameters)
        {
            DataTable templates = new DataTable();
            templates = this.ReturnDSRecords(query, parameters);

            _availableTamplates.Clear();

            foreach (DataRow r in templates.Rows)
            {
                _availableTamplates.Add(new KeyValuePair<string, string>(r["ID"].ToString(), r["HName"].ToString()));
            }

            OnPropChanged(nameof(this.AvailableTemplates));
        }
    }
}
