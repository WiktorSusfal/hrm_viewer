using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_Viewer.Utilities;

namespace HRM_Viewer.Models
{
    public class AppException : ObservableObject
    {
        private string _exMsg;
        public string ExMsg
        {
            get => _exMsg;
            set
            {
                if (value.ToString() != null)
                {
                    _exMsg = value.ToString();
                }
                OnPropChanged(nameof(this.ExMsg));
            }
        }

        private int _operationStatus;
        public int OperationStatus
        {
            get => _operationStatus;
            set
            {
                 _operationStatus = value;
                OnPropChanged(nameof(this.OperationStatus));
            }
        }
    }
}

