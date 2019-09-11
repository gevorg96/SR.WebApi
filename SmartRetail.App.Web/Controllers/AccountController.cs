using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using SmartRetail.App.Web.Models;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Auth;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    public class AccountController : ControllerBase
    {
        IUserRepository _repo;

        public AccountController(IUserRepository repo)
        {
            _repo = repo;
        }

        //private List<User> people = new List<User>
        //{
        //    new User {Username="admin@gmail.com", Password="12345", Role = "admin" },
        //    new User { Username="qwerty", Password="55555", Role = "user" }
        //};
 
        [HttpPost("/login")]
        public async Task<ActionResult> Token([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var identity = await GetIdentity(model.username, model.password);
            if (identity == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
             
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
 
            return Ok(response);
        }
 
        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var user = await _repo.GetByLogin(username);
            if (user != null)
            {
                var passwordValid = Crypto.VerifyHashedPassword(user.Password, password);
                if (!passwordValid)
                {
                    return null;
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
                };
                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}