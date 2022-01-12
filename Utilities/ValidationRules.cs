using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HRM_Viewer.Utilities
{
    public abstract class BaseValidationRule : ValidationRule
    {
        public List<string> forbiddenWords = new List<string>()
            { ";", ",", ".", " SELECT ", " SELECT", " INSERT ", " INSERT", " UNION ", " UNION", " PROCEDURE ", " PROCEDURE", " PROC ", " PROC", " DELETE ", " DELETE",
                " DROP ", " DROP", " TRUNCATE ", " TRUNCATE", " EXECUTE ", "EXECUTE", " EXEC ", " EXEC", " OR ", " XOR ", " AND " };

        public BaseValidationRule()
        { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(false, $"Base validation called. Default invalid value returned.");
        }

        public bool IsNullValue(object value)
        {
            return value == null;
        }

        public bool IsInvalidStringValue(object value)
        {
            try
            {
                string tmp = value.ToString();
                return false;
            }
            catch
            {
                return true;
            }
        }

        public bool IsEmptyString(object value)
        {
            return value.ToString() == string.Empty;
        }

        public string ContainForbiddenWords(object value)
        {
            string input = value.ToString();

            foreach (string word in forbiddenWords)
            {
                if (input.ToLower().Contains(word.ToLower()))
                {
                    return word;
                }
            }

            return null;
        }

        public bool InNotNumericGreaterThanZero(object value)
        {
            string strInput = value.ToString();
            int input = -1;

            try
            {
                input = int.Parse(strInput);
                if (input < 0)
                    return true;
            }
            catch
            {
                return true;
            }

            return false;
        }
    }
    public class MainSettingRule : BaseValidationRule
    {
        public MainSettingRule()
        { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string forbiddenWord;

            try
            {
                if (IsNullValue(value))
                    return new ValidationResult(false, $"Cannot accept null value!");
                if (IsInvalidStringValue(value))
                    return new ValidationResult(false, $"Cannot convert to string the value provided!");
                if (IsEmptyString(value))
                    return new ValidationResult(false, $"Cannot accept empty strings!");

                forbiddenWord = ContainForbiddenWords(value);
                if (forbiddenWord != null)
                    return new ValidationResult(false, $"Cannot accept following word in provided value: {forbiddenWord}");
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Error occured during validation. Details: {e.Message} ");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class MainOptionalSettingRule : BaseValidationRule
    {
        public MainOptionalSettingRule()
        { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string forbiddenWord;

            try
            {
                if (IsNullValue(value))
                    return new ValidationResult(false, $"Cannot accept null value!");
                if (IsInvalidStringValue(value))
                    return new ValidationResult(false, $"Cannot convert to string the value provided!");

                forbiddenWord = ContainForbiddenWords(value);
                if (forbiddenWord != null)
                    return new ValidationResult(false, $"Cannot accept following word in provided value: {forbiddenWord}");
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Error occured during validation. Details: {e.Message} ");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class NotEmptyPasswordRule : BaseValidationRule
    {
        public NotEmptyPasswordRule()
        { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (IsNullValue(value))
                    return new ValidationResult(false, $"Cannot accept null value!");
                if (IsInvalidStringValue(value))
                    return new ValidationResult(false, $"Cannot convert to string the value provided!");
                if (IsEmptyString(value))
                    return new ValidationResult(false, $"Cannot accept empty strings!");
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Error occured during validation. Details: {e.Message} ");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class MustBeNumberGTZeroRule : BaseValidationRule
    {
        public MustBeNumberGTZeroRule() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            try
            {
                if (IsNullValue(value))
                    return new ValidationResult(false, $"Cannot accept null value!");
                if (IsInvalidStringValue(value))
                    return new ValidationResult(false, $"Cannot convert to string the value provided!");
                if (IsEmptyString(value))
                    return new ValidationResult(false, $"Cannot accept empty strings!");
                if (InNotNumericGreaterThanZero(value))
                    return new ValidationResult(false, $"Cannot accept non numeric input or values less than 0! You entered: {value.ToString()}");
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Error occured during validation. Details: {e.Message} ");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class FullDSNameRule : MainSettingRule
    {
        public FullDSNameRule()
        {
            forbiddenWords = new List<string> ()
                { ";", ",", " SELECT ", " SELECT", " INSERT ", " INSERT", " UNION ", " UNION", " PROCEDURE ", " PROCEDURE", " PROC ", " PROC", " DELETE ", " DELETE",
                " DROP ", " DROP", " TRUNCATE ", " TRUNCATE", " EXECUTE ", "EXECUTE", " EXEC ", " EXEC", " OR ", " XOR ", " AND " };
        }
    }
}
