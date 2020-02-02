using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClearlyApi;
using clearlyApi.Dto.Request;
using clearlyApi.Dto.Response;
using ClearlyApi.Entities;
using ClearlyApi.Enums;
using ClearlyApi.Services.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace clearlyApi.Controllers
{
    [Route("api/[controller]")]
    public class ChatController : Controller
    {

        private ApplicationContext _dbContext { get; set; }
        private ChatMessageHandler _webSocketHandler { get; set; }

        public ChatController(ApplicationContext dbContext, ChatMessageHandler webSocketHandler)
        {
            this._dbContext = dbContext;
            _webSocketHandler = webSocketHandler;
        }

        [Authorize]
        [HttpPost("sendPhoto")]
        public async Task<IActionResult> SendPhoto(IFormFile file)
        {
            var user = _dbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            if (file == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "File empty"
                });

            var admin = _dbContext.Users
               .FirstOrDefault(x => x.UserType == UserType.Admin);

            if (admin == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "Admin not found"
                });

            string fileName = $"{CryptHelper.CreateMD5(DateTime.Now.ToString())}{Path.GetExtension(file.FileName)}";
            string path = $"{Directory.GetCurrentDirectory()}/Files/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (var fileStream = new FileStream(path + fileName, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var message = new Message
            {
                Type = MessageType.Photo,
                Content = fileName,
                Created = DateTime.UtcNow,
                UserId = user.Id,
                AdminId = admin.Id,
                IsFromAdmin = false
            };

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            SendMessageSocket(user.Login, new MessageDTO(message) { Data = fileName });

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpPost("adminSendPhoto/{toLogin}")]
        public async Task<IActionResult> AdminSendPhoto(IFormFile file, string toLogin)
        {
            var user = _dbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null || user.UserType != UserType.Admin)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            var toUser = _dbContext.Users
                .FirstOrDefault(x => x.Login == toLogin);
            if(toUser == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "To User not found"
                });


            if (file == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "File empty"
                });

            string fileName = $"{CryptHelper.CreateMD5(DateTime.Now.ToString())}{Path.GetExtension(file.FileName)}";
            string path = $"{Directory.GetCurrentDirectory()}\\Files\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (var fileStream = new FileStream(path + fileName, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var message = new Message
            {
                Type = MessageType.Photo,
                Content = fileName,
                Created = DateTime.UtcNow,
                UserId = toUser.Id,
                AdminId = user.Id,
                IsFromAdmin = true
            };

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            SendMessageSocket(toUser.Login, new MessageDTO(message) { Data = fileName});


            var agePickerMessage = new Message
            {
                Type = MessageType.AgePicker,
                Created = DateTime.UtcNow,
                UserId = toUser.Id,
                AdminId = user.Id,
                IsFromAdmin = true
            };
            _dbContext.Messages.Add(agePickerMessage);
            _dbContext.SaveChanges();

            SendMessageSocket(toUser.Login, new MessageDTO(agePickerMessage));

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpGet("getMessages")]
        public IActionResult GetMessages(
            [FromQuery(Name = "pageNumber")] int pageNumber,
            [FromQuery(Name = "pageSize")] int pageSize
            )
        {
            if (pageNumber < 1)
                pageNumber = 1;

            var user = _dbContext.Users
                .Include(u => u.Person)
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            var messages = _dbContext.Messages
                .Where(m => m.UserId == user.Id)
                .ToList();

            var result = new List<MessageDTO>();

            foreach(var item in messages)
            {
                if (item.Type == MessageType.PackagesPicker)
                {
                    var packages = _dbContext.Packages
                                        .Include(x => x.Title)
                                        .Include(x => x.Description)
                                        .Take(3).ToList();

                    result.Add(new MessageDTO(item, packages));
                }
                else result.Add(new MessageDTO(item));
            }

            return Json(new DataResponse<MessageDTO>
            {
                Data = result
            });
        }




        [Authorize]
        [HttpPost("message")]
        public IActionResult SendMessage([FromBody] MessageRequest request)
        {
            var user = _dbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            if (request == null)
                return Json(new { Status = false, Message = "Request cannot be null" });


            var message = new Message
            {
                Type = MessageType.Text,
                Content = request.Text,
                Created = DateTime.UtcNow
            };
            string receiverLogin = "";

            if (user.UserType == UserType.Admin)
            {
                var toUser = _dbContext.Users
                .FirstOrDefault(x => x.Login == request.ToUserLogin);

                if (toUser == null)
                    return Json(new BaseResponse
                    {
                        Status = false,
                        Message = "User not found"
                    });

                message.AdminId = user.Id;
                message.UserId = toUser.Id;
                message.IsFromAdmin = true;

                receiverLogin = toUser.Login;
            }
            else
            {
                var admin = _dbContext.Users
                    .FirstOrDefault(x => x.UserType == UserType.Admin);

                if (admin == null)
                    return Json(new BaseResponse
                    {
                        Status = false,
                        Message = "Admin not found"
                    });

                message.UserId = user.Id;
                message.AdminId = admin.Id;
                receiverLogin = admin.Login;
            }

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            SendMessageSocket(receiverLogin, new MessageDTO(message) { Data = message.Content});

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpGet("setAge")]
        public IActionResult SetAge([FromQuery(Name = "age")] string ageInterval)
        {
            var user = _dbContext.Users
                .Include(u => u.Person)
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            var lastMessage = _dbContext.Messages
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
            if (lastMessage == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "Отправьте фото"
                });

            if(String.IsNullOrEmpty(ageInterval))
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "возраст не задан"
                });

            user.Person.Age = ageInterval;

            var message = new Message
            {
                Type = MessageType.PaymentMethodPicker,
                UserId = user.Id,
                AdminId = lastMessage.AdminId,
                Created = DateTime.UtcNow
            };

            _dbContext.Messages.Add(message);

            SendMessageSocket(user.Login, new MessageDTO(message));


            _dbContext.SaveChanges();

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpGet("setPayType/{type}")]
        public IActionResult SetPayType(PayType type)
        {
            var user = _dbContext.Users
               .Include(u => u.Person)
               .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            var orderExpired = _dbContext.Orders
                .FirstOrDefault(x => x.UserId == user.Id && x.Status == OrderStatus.Request);

            if(orderExpired != null)
                _dbContext.Orders.Remove(orderExpired);
    
            var order = new Order()
            {
                UserId = user.Id,
                Created = DateTime.UtcNow,
                Status = type == PayType.Cash ? OrderStatus.Cash : OrderStatus.Request
            };

            _dbContext.Orders.Add(order);

            _dbContext.SaveChanges();

            var lastMessage = _dbContext.Messages
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            var packages = _dbContext.Packages
                .Include(x => x.Title)
                .Include(x => x.Description)
                .Take(3).ToList();

            var message = new Message
            {
                Type = MessageType.PackagesPicker,
                Content = order.Id.ToString(),
                IsFromAdmin = true,
                UserId = user.Id,
                AdminId = lastMessage.AdminId,
                Created = DateTime.UtcNow
            };

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            var packagesListMessage = new MessageDTO(message)
            {
                Data = JsonConvert.SerializeObject(
                    new PackagesList()
                    {
                        OrderId = order.Id,
                        Packages = packages.Select(x => new PackageDTOResponse(x)).ToList()
                    }
                )
            };

            SendMessageSocket(user.Login, packagesListMessage);

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpPost("setPackage")]
        public IActionResult SetPackage([FromBody] PackageRequestDTO request)
        {

            if (request == null)
                return Json(new { Status = false, Message = "Request cannot be null" });

            if (!Validator.TryValidateObject(request, new ValidationContext(request), null, true))
                return Json(new { Status = false, Message = "Required Property Not Found" });


            var user = _dbContext.Users
               .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            var pack = _dbContext.Packages.Find(request.PackageId);
            if(pack == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "Package not found"
                });

            var order = _dbContext.Orders.Find(request.OrderId);
            if (order == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "Order not found"
                });

            order.PackageId = pack.Id;

            _dbContext.SaveChanges();

            return Json(new BaseResponse());
        }

        private async Task SendMessageSocket(string login, MessageDTO message)
        {
            await _webSocketHandler.SendMessageAsync(
                    login,
                    JsonConvert.SerializeObject(message)
                    );
        }


    }
}
