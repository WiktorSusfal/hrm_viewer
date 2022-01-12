using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Viewer.Utilities
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropChanged(string setting)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(setting));
            }
        }
    }

    public class ValidatableObject : ObservableObject, INotifyDataErrorInfo
    {
        //Error notyfying
        public Dictionary<string, List<string>> errors;
        public Dictionary<string, List<ValidationRule>> validationRules;
        private object syncLock = new object();

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !errors.ContainsKey(propertyName))
            {
                return null;
            }

            return errors[propertyName];
        }

        public bool HasErrors
        {
            get 
            {
                if (errors == null)
                    return false;
                else
                    return errors.Count > 0; 
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public void OnErrorsChanged(string error)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(error));
                OnPropChanged("HasErrors");
            }
        }

        public void AddError(string propertyName, string error, bool isWarning)
        {
            if (!this.errors.ContainsKey(propertyName))
            {
                this.errors[propertyName] = new List<string>();
            }

            if (!this.errors[propertyName].Contains(error))
            {
                if (isWarning)
                {
                    this.errors[propertyName].Add(error);
                }
                else
                {
                    this.errors[propertyName].Insert(0, error);
                }
                OnErrorsChanged(propertyName);

            }
        }

        public void RemoveError(string propertyName, string error)
        {
            if (this.errors.ContainsKey(propertyName))
            {
                if (this.errors[propertyName].Contains(error))
                {
                    this.errors[propertyName].Remove(error);
                    if (this.errors[propertyName].Count == 0)
                    {
                        this.errors.Remove(propertyName);
                    }
                    OnErrorsChanged(propertyName);

                }

            }
        }

        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock (this.syncLock)
            {
                //IF THERE IS NO VALIDATION RULE FOR GIVEN PROPERTY IN THAT CLASS, RETURN
                if (!this.validationRules.TryGetValue(propertyName, out List<ValidationRule> propertyValidationRules))
                {
                    return;
                }

                //CLEAR PREVIOUS ERRORS FROM TESTED PROPERTY
                if (this.errors.ContainsKey(propertyName))
                {
                    this.errors.Remove(propertyName);
                    OnErrorsChanged(propertyName);
                }

                propertyValidationRules.ForEach(
                    ValidationRule =>
                    {
                        ValidationResult VResult = ValidationRule.Validate(value, CultureInfo.CurrentCulture);
                        if (!VResult.IsValid)
                            AddError(propertyName, VResult.ErrorContent.ToString(), false);
                    });

            }
        }
    }

}
