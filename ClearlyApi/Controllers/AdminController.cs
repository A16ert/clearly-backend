using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClearlyApi;
using clearlyApi.Dto.Request;
using clearlyApi.Dto.Response;
using ClearlyApi.Entities;
using ClearlyApi.Enums;
using ClearlyApi.Services.Auth;
using ClearlyApi.Services.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Utils;

namespace clearlyApi.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        
        private ApplicationContext DbContext { get; set; }
        private IAuthService AuthService { get; set; }

        private ChatMessageHandler WebSocketHandler { get; set; }
        
        public AdminController(
            ApplicationContext dbContext,
            IAuthService authService,
            ChatMessageHandler webSocketHandler
            )
        {
            DbContext = dbContext;
            AuthService = authService;
            
            WebSocketHandler = webSocketHandler;
        }

        [HttpPost("loginAdminTest")]
        public IActionResult AdminAuthOrRegister([FromBody] AuthRequest request)
        {
            if (request == null)
                return Json(new {Status = false, Message = "Request cannot be null"});

            if (!Validator.TryValidateObject(request, new ValidationContext(request), null, true))
                return Json(new {Status = false, Message = "Required Property Not Found"});

            if (request.Code != "12345")
                return Json(new {Status = false, Message = "неверный пароль"});

            var user = DbContext.Users.FirstOrDefault(x => x.Login == request.Login);
            if (user == null)
            {
                user = new User()
                {
                    Login = request.Login,
                    LoginType = LoginType.Email,
                    UserType = UserType.Admin,
                    IsActive = true,
                    Created = DateTime.Now
                };
                DbContext.Users.Add(user);
                DbContext.SaveChanges();
            }
            // если пользователь не админ
            else if (user.UserType == UserType.User)
                return Json(new {Status = false, Message = "Пользователь не найден"});

            DbContext.SaveChanges();

            var token = AuthService.CreateToken(user);

            return Json(new SignInResponse() {SecurityToken = token, Id = user.Id});
        }

        [Authorize]
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var user = DbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name && x.UserType == UserType.Admin);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });


            var users = DbContext.Users.Select(u => new UserResponseDTO(u)).ToList();

            return Json(users);
        }

        [Authorize]
        [HttpGet("notifications")]
        public IActionResult GetAllNotificatoins()
        {
            var user = DbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name && x.UserType == UserType.Admin);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });


            var notifications = DbContext.Notifications.Select(u => new NotificationResponseDto(u)).ToList();

            return Json(new DataResponse<NotificationResponseDto>()
            {
                Data = notifications
            });
        }

        [Authorize]
        [HttpGet("orders")]
        public IActionResult GetAllOrders()
        {
            var user = DbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name && x.UserType == UserType.Admin);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });


            var orders = DbContext.Orders
                .Select(u => new OrderResponseDTO(u))
                .ToList();

            return Json(new DataResponse<OrderResponseDTO>()
            {
                Data = orders
            });
        }
        
        [Authorize]
        [HttpPost("sendPhoto/{toLogin}")]
        public async Task<IActionResult> SendPhoto(IFormFile file, string toLogin)
        {
            var user = DbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name && x.UserType == UserType.Admin);

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

            var toUser = DbContext.Users
                .FirstOrDefault(x => x.Login == toLogin);
            if (toUser == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "To User not found"
                });

            var fileName = $"{CryptHelper.CreateMD5(DateTime.Now.ToString())}{Path.GetExtension(file.FileName)}";
            var path = $"{Directory.GetCurrentDirectory()}\\Files\\";

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

            DbContext.Messages.Add(message);
            DbContext.SaveChanges();

            SendMessageSocket(toUser.Login, new MessageDTO(message) { Data = fileName});

            var notification = DbContext.Notifications.FirstOrDefault(x => x.UserId == toUser.Id && x.Type == NotificationType.Photo);

            if (notification != null)
            {
                DbContext.Remove(notification);
                DbContext.SaveChanges();
            }
            
            var agePickerMessage = new Message
            {
                Type = MessageType.AgePicker,
                Created = DateTime.UtcNow,
                UserId = toUser.Id,
                AdminId = user.Id,
                IsFromAdmin = true
            };
            DbContext.Messages.Add(agePickerMessage);
            DbContext.SaveChanges();

            SendMessageSocket(toUser.Login, new MessageDTO(agePickerMessage));

            return Json(new BaseResponse());
        }

        [Authorize]
        [HttpPost("sendMessage")]
        public IActionResult SendMessage([FromBody] MessageRequest request)
        {
            var user = DbContext.Users
                .FirstOrDefault(x => x.Login == User.Identity.Name && x.UserType == UserType.Admin);
            
            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });
            
            if (request == null)
                return Json(new { Status = false, Message = "Request cannot be null" });
            
            var toUser = DbContext.Users
                .FirstOrDefault(x => x.Login == request.ToUserLogin);
            
            if (toUser == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });
            
            var message = new Message
            {
                Type = MessageType.Text,
                Content = request.Text,
                
                AdminId = user.Id,
                UserId = toUser.Id,
                IsFromAdmin = true,
                
                Created = DateTime.UtcNow
            };

            DbContext.Messages.Add(message);
            DbContext.SaveChanges();

            var notification = DbContext.Notifications.FirstOrDefault(x => x.UserId == toUser.Id && x.Type == NotificationType.Message);
            if (notification != null)
            {
                DbContext.Remove(notification);
                DbContext.SaveChanges();
            }
            
            SendMessageSocket(toUser.Login, new MessageDTO(message) { Data = message.Content});
            
            return Json(new BaseResponse());
        }
        
        private async Task SendMessageSocket(string login, MessageDTO message)
        {
            await WebSocketHandler.SendMessageAsync(
                login,
                JsonConvert.SerializeObject(message)
            );
        }
    }
}