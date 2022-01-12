using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Viewer.Services
{
    public interface ISQLDataAccess
    {
        void TestConnection();
        DataTable ReturnDSRecords(string query, Dictionary<string, string> parameters);
        DataTable ReturnDSSchema(string query, Dictionary<string, string> paramerers);

    }
}
