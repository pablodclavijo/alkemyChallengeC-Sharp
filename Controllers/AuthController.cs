using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AlkemyChallenge.Models;
using NHibernate.Mapping;
using AlkemyChallenge.ViewModels.Auth;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AlkemyChallenge.Services.SendGrid;

namespace AlkemyChallenge.Controllers
{
    [Route("/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;


        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterRequestModel model)
        {
            var userExists = await userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return BadRequest();
            }
            var user = new User
            {
                Email = model.Email,
                UserName = model.Username,
                IsActive = true
            };
            var userCreated = await userManager.CreateAsync(user, model.Password);
            if (!userCreated.Succeeded)
            {
                return StatusCode(500,
                    new
                    {
                        Status = "Internal error",
                        Message = $"User creation failed, " + $"errors: {String.Join(", ", userCreated.Errors.Select(e => e.Description))}"
                    });
            }
            Sendgrid.sendEmail(model.Email, model.Username).Wait();
            return Ok(
                new
                {
                    Status = "Success",
                    Message = "User succesfully created"
                }
                );
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var userValidated = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (!userValidated.Succeeded)
            {
                return BadRequest(
                    new
                    {
                        Status = "Bad Request",
                        Message = "validation failed"
                    });

            }
            var user = await userManager.FindByNameAsync(model.Username);
            if (!user.IsActive)
            {
                return Unauthorized(
                    new
                    {
                        Status = "Unauthorized",
                        Message = "Username not found"
                    });
            }
            var token = await GetToken(user);
            return Ok(new
            {
                token= new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
     private async Task<JwtSecurityToken> GetToken(User currentUser)
        {
            var userRol = await userManager.GetRolesAsync(currentUser);
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, currentUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            authClaims.AddRange(userRol.Select(e => new Claim(ClaimTypes.Role, e)));
            var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret1234567890"));
            var token = new JwtSecurityToken(
                issuer: "http://localhost:7142",
                audience: "http://localhost:7142",
                expires: DateTime.Now.AddMinutes(15),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
