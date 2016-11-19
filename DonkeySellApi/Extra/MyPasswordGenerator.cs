using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using Microsoft.Ajax.Utilities;

namespace DonkeySellApi.Extra
{
    public interface IMyPasswordGenerator
    {
        string GeneratePassword();
    }

    public class MyPasswordGenerator: IMyPasswordGenerator
    {
        public string GeneratePassword()
        {
            var seed = DateTime.Now.Millisecond;
            var password = Membership.GeneratePassword(15, 0);
            password = Regex.Replace(password, @"[^a-zA-Z0-9]", m => new Random(seed).Next(0, 9).ToString());
           
            for (int i = 0; i <= password.Length; i++)
            {
                if (Char.IsLetter(password[i]))
                {
                    password = password.Replace(password[i], Char.ToUpper(password[i]));
                    break;
                }
            }

            return password.Substring(0, 9);
        }
    }
}