using System;

namespace ClearlyApi.Enums.Converter
{
    public static class NotificationTypeConverter
    {
        public static string ToString(NotificationType type)
        {
            string typeText;
            switch (type)
            {
                case NotificationType.Message:
                    typeText = "сообщение";
                    break;
                case NotificationType.Photo:
                    typeText = "фото";
                    break;
                case NotificationType.OrderCreated:
                    typeText = "заказ был создан";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            return typeText;
        }
    }
}