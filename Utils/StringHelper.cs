using System;
using System.Linq;

namespace Utils
{
    public static class StringHelper
    {

        /// <summary>
        /// Returns only Digits From String
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        ///https://stackoverflow.com/a/3977549
        public static string GetNumbers(this string str)
        {
            return new string(str.Where(char.IsDigit).ToArray());
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

