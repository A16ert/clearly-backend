using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ClearlyApi;
using clearlyApi.Dto.Request;
using clearlyApi.Dto.Response;
using ClearlyApi.Entities;
using ClearlyApi.Enums;
using ClearlyApi.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace clearlyApi.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        
        private ApplicationContext DbContext { get; set; }
        private IAuthService AuthService { get; set; }

        public AdminController(ApplicationContext dbContext, IAuthService authService)
        {
            this.DbContext = dbContext;
            this.AuthService = authService;
        }
        
        [HttpPost("loginAdminTest")]
        public IActionResult AdminAuthOrRegister([FromBody] AuthRequest request)
        {
            if (request == null)
                return Json(new { Status = false, Message = "Request cannot be null" });

            if (!Validator.TryValidateObject(request, new ValidationContext(request), null, true))
                return Json(new { Status = false, Message = "Required Property Not Found" });

            if(request.Code != "12345")
                return Json(new { Status = false, Message = "неверный пароль" });
            
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
            else if(user.UserType == UserType.User)
                    return Json(new { Status = false, Message = "Пользователь не найден" });

            DbContext.SaveChanges();

            var token = AuthService.CreateToken(user);

            return Json(new SignInResponse() { SecurityToken = token, Id = user.Id });
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
            
            return Json(new DataResponse<UserResponseDTO>()
            {
                Data = users
            });
        }
    }
}