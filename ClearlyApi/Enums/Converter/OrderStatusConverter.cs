using System;

namespace ClearlyApi.Enums.Converter
{
    public static class OrderStatusConverter
    {
        public static string ToString(OrderStatus type)
        {
            string typeText;
            switch (type)
            {
                case OrderStatus.Request:
                    typeText = "Адрес доставки не указан";
                    break;
                case OrderStatus.AwaitDelivery:
                    typeText = "Ожидает начала доставки";
                    break;
                case OrderStatus.IsDelivered:
                    typeText = "В пути";
                    break;
                case OrderStatus.Delivered:
                    typeText = "Доставлен";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return typeText;
        }
    }
}