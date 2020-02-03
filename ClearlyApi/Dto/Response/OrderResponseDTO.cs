using System;
using ClearlyApi.Entities;
using ClearlyApi.Enums;
using ClearlyApi.Enums.Converter;
using Microsoft.EntityFrameworkCore;

namespace clearlyApi.Dto.Response
{
    public class OrderResponseDTO : BaseResponse
    {
        public OrderResponseDTO()
        {
        }

        public OrderResponseDTO(Order model)
        {
            if(model.Package == null || model.Delivery == null || model.User == null)
                throw new Exception();
            OrderId = model.Id;
            Login = model.User.Login;
            
            PackageName = model.Package.Title;
            Amount = model.Package.Price;
            
            Status = model.Status;
            StatusText = OrderStatusConverter.ToString(model.Status);
            Updated = model.Updated;
            
            var delivery = model.Delivery;
            
            Address = $"{delivery.City} ул. {delivery.Street} д. {delivery.HouseNumber} кв. {delivery.Apartment}";
            PhoneNumber = delivery.PhoneNumber;

        }
        public int OrderId { get; set; }
        
        public string Login { get; set; }
        
        public string PackageName { get; set; }
        
        public int Amount { get; set; }
        
        public OrderStatus Status { get; set; }
        
        public string StatusText { get; set; }
        
        public string Address { get; set; }
        
        public string PhoneNumber { get; set; }
        public DateTime Updated { get; set; }
    }
}
