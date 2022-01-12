using HRM_Viewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HRM_Viewer.Utilities
{
    public class RelayCommand : ICommand
    {
        private AppException _appEx;
        public AppException AppEx
        {
            private set { _appEx = value; }
            get { return _appEx; }
        }

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public RelayCommand(ref AppException appEx, Action<object> execute, Predicate<object> canExecute = null)
        {
            if (execute == null)
                throw new NullReferenceException("execute");

            _execute = execute;
            _canExecute = canExecute;

            if (appEx == null)
                throw new NullReferenceException("appEx");
            this.AppEx = appEx;
        }
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            string msg = "Operation succeeded!";
            int status = 1;
            try
            {
                _execute.Invoke(parameter);
            }
            catch(Exception e)
            {
                msg = $"Operation failed - details: {e.Message}";
                status = -1;
            }

            AppEx.ExMsg = msg;
            AppEx.OperationStatus = status;      
        }
    }
}
