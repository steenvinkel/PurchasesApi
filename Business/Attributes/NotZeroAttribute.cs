using System;
using System.ComponentModel.DataAnnotations;

namespace Business.Attributes
{
    public class NotZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue != 0;
            }
            if (value is int intValue)
            {
                return intValue != 0;
            }

            throw new NotImplementedException("NotZeroAttribute is only implemented for decimal and int types");
        }   

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot be zero.";
        }
    }
}
