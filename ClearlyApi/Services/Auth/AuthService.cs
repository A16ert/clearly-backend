using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using clearlyApi.Dto;
using clearlyApi.Dto.Response;
using ClearlyApi.Entities;
using ClearlyApi.Enums;
using Microsoft.IdentityModel.Tokens;
using SmsSender;

namespace ClearlyApi.Services.Auth
{
    public class AuthService : IAuthService
    {
        private ApplicationContext DbContext { get; set; }
        private ISmsProvider SmsProvider { get; set; }

        private EmailService EmailService { get; set; }
        public AuthService(ApplicationContext applicationContext, ISmsProvider smsProvider)
        {
            DbContext = applicationContext;
            this.SmsProvider = smsProvider;
            this.EmailService = new EmailService();
        }

        public BaseResponse Auth(User user)
        { 
            var loginType = user.LoginType;

            Random rnd = new Random();
            int randomNumber = rnd.Next(100000, 999999);
            try
            {
                switch (loginType)
                {
                    case LoginType.Email:
                        var toEmail = user.Login;
                        EmailService.SendEmailAsync(toEmail, "Cleanly код активации", randomNumber.ToString());
                        break;
                    case LoginType.Phone:
                        SmsProvider.Send(null, user.Login, randomNumber.ToString());
                        break;
                }
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException is System.IO.IOException)
                {

                }
            }
            ActivationCode code = new ActivationCode
            {
                User = user,
                Created = DateTime.UtcNow,
                Code = randomNumber.ToString()
            };
            DbContext.ActivationCodes.Add(code);
            DbContext.SaveChanges();

            return new BaseResponse();
        }

        public BaseResponse Register(string login, LoginType loginType)
        {

            Random rnd = new Random();
            int randomNumber = rnd.Next(100000, 999999);

            try
            {
                switch (loginType)
                {
                    case LoginType.Email:
                        var toEmail = login;
                        EmailService.SendEmailAsync(toEmail, "Cleanly код активации", randomNumber.ToString());
                        break;
                    case LoginType.Phone:
                        SmsProvider.Send(null, login, randomNumber.ToString());
                        break;
                }
            }
            catch(System.Exception ex)
            {
                if(ex.InnerException is System.IO.IOException)
                {

                }
            }

            var user = new User()
            {
                LoginType = loginType,
                Login = login,
                Created = DateTime.UtcNow
            };

            DbContext.Users.Add(user);
            DbContext.SaveChanges();

            var person = new Person()
            {
                UserId = user.Id
            };

            switch (loginType)
            {
                case LoginType.Email:
                    person.Email = login;
                    break;
                case LoginType.Phone:
                    person.Phone = login;
                    break;
            }

            DbContext.Persons.Add(person);
            DbContext.SaveChanges();

            ActivationCode code = new ActivationCode
            {
                User = user,
                Created = DateTime.UtcNow,
                Code = randomNumber.ToString()
            };
            DbContext.ActivationCodes.Add(code);
            DbContext.SaveChanges();

            return new BaseResponse();
        }

        public SecurityTokenViewModel CreateToken(User user)
        {
            var identity = GetIdentity(user);
            var token = GetSecurityToken(identity, false);

            var expiredToken = DbContext.AccountSessions
                .FirstOrDefault(x => x.UserId == user.Id && x.ExpiredAt >= token.ExpireDate);

            if (expiredToken != null)
            {
                expiredToken.ExpiredAt = DateTime.UtcNow;
            }

            var accSession = new AccountSession
            {
                UserId = user.Id,
                Created = DateTime.UtcNow,
                ExpiredAt = token.ExpireDate,
                Token = token.Token
            };
            DbContext.AccountSessions.Add(accSession);
            DbContext.SaveChanges();

            return token;
        }

        private ClaimsIdentity GetIdentity(User user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        private SecurityTokenViewModel GetSecurityToken(ClaimsIdentity identity, bool remember)
        {
            var now = DateTime.UtcNow;
            var expires = now;

            if (remember)
                expires = DateTime.MaxValue;
            else
                expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME));

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(
                        AuthOptions.GetSymmetricSecurityKey(),
                        SecurityAlgorithms.HmacSha256)
                    );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var token = new SecurityTokenViewModel()
            {
                Token = encodedJwt,
                ExpireDate = expires
            };

            return token;
        }
    }
}
