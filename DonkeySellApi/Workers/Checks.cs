using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.ViewModels;

namespace DonkeySellApi.Workers
{
    public static class Checks
    {
        public static bool IsValid(this ViewUser viewUser)
        {
            bool isEmail = Regex.IsMatch(viewUser.Email,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
            bool usernameLengthOk = viewUser.UserName.Length >= 4 && viewUser.UserName.Length < 16;
            bool notOverflowImage = true;
            if (viewUser.Avatar != null)
                notOverflowImage = System.Text.Encoding.ASCII.GetByteCount(viewUser.Avatar) < 2500000;

            return isEmail == usernameLengthOk == notOverflowImage;
        }

        public static bool PasswordIsValid(string password)
        {
            bool lengthOk = password.Length <= 16 && password.Length >= 8;
            bool containsUppercase = password.Any(char.IsUpper);
            bool containsNumeric = password.Any(char.IsNumber);
            bool containsEmptySpace = password.Any(char.IsWhiteSpace);

            return lengthOk == containsUppercase == containsNumeric == !containsEmptySpace;
        }

        public static bool IsValid(this Product product)
        {
            bool onlyNumericPrice = product.Price.ToString().Any(char.IsLetter);
            bool hasDescription = product.Description.Length >= 5;

            return !onlyNumericPrice == hasDescription;
        }

        public static bool IsValid(this Improvement improvement)
        {
            bool hasValue = !string.IsNullOrWhiteSpace(improvement.Value);

            return hasValue;
        }

        public static bool IsValid(this Message message)
        {
            bool messageHasValue = !string.IsNullOrWhiteSpace(message.Value);
            
            return messageHasValue;
        }
    }
}