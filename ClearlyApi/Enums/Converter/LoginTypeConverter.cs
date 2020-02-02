using System;

namespace ClearlyApi.Enums.Converter
{
    public static class LoginTypeConverter
    {
        public static string ToString(LoginType type)
        {
            string typeText;
            switch (type)
            {
                case LoginType.Google:
                    typeText = "Google";
                    break;
                case LoginType.Phone:
                    typeText = "Phone";
                    break;
                case LoginType.Email:
                    typeText = "Email";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return typeText;
        }
    }
}