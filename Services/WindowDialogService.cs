using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HRM_Viewer.Services
{
    class WindowDialogService : IDialogService
    {
        public string SelectFolderPath()
        {
            var d = new FolderBrowserDialog();

            if (d.ShowDialog() == DialogResult.OK)
                return d.SelectedPath;

            return null;
        }
    }
}
