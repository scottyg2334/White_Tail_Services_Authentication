using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AuthorizationServer.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using AuthorizationServer.DbContext;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using AuthorizationServer.Services;
using Npgsql;
using AuthorizationServer.Helpers;

namespace AuthorizationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AuthContext _context;
        private readonly ITokenManager _tokenManager;
        private readonly IBrokerService _brokerService;

        public AuthController(IConfiguration config, AuthContext context, ITokenManager tokenManager,
            IBrokerService brokerService)
        {
            _brokerService = brokerService;
            _config = config;
            _context = context;
            _tokenManager = tokenManager;
        }

        [HttpPost("registeruser")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]Models.AuthUserModel authModel)
        {
            IActionResult response = Unauthorized();
           AuthUserModel user = _context.AuthUsers.Where(u => u.Email == authModel.Email && u.Password == authModel.Password).FirstOrDefault();


            if (user == null)
            {   
                authModel.GUID = Guid.NewGuid();
                _context.Add<AuthUserModel>(authModel);
                _context.SaveChanges();
                response = Ok();
                // Add Logic to call UsersService to create new entry for this newly reigsted user. 
                User newUser = new User() {Guid = authModel.GUID, Email = authModel.Email};
                _brokerService.SetMessage(newUser);
                _brokerService.PublishMessageNewUserCreated();
                return response;

            }
/*             else
            {
                Console.WriteLine("User already exists.");
            } */

            return response;
            
        }        


        [HttpPost("loginuser")]
        [AllowAnonymous]
        public IActionResult Login([FromBody]Models.AuthUserModel authModel)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(authModel);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
            
        }
        [HttpGet("currentuser")]
        public Task<ActionResult<AuthUserModel>> GetCurrentUser()
        {
            AuthUserModel currentUser = new AuthUserModel();
            if (User.Identity.IsAuthenticated)
            {
                currentUser.Email = User.FindFirstValue(ClaimTypes.Email);
            }
            return Task.FromResult<ActionResult<AuthUserModel>>(currentUser);
        } 
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeAccessToken()
        {
            await _tokenManager.DeactivateCurrentAsync();

            return NoContent();
        }
        

        public  AuthUserModel AuthenticateUser(AuthUserModel authModel)
        {
           AuthUserModel user = _context.AuthUsers.Where(u => u.Email == authModel.Email && u.Password == authModel.Password).FirstOrDefault();

            if (user == null)
            {
                return user;
            }
            return  user;
        }

        public string GenerateJSONWebToken(AuthUserModel authModel)
        {

            // Key Needs to be Encypted. 
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity (new Claim[]
                {
                    new Claim("ID", authModel.GUID.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, authModel.Email),
                    new Claim(ClaimTypes.Role, "Default"),
                    new Claim(ClaimTypes.NameIdentifier, authModel.GUID.ToString())

                }),
                Expires = DateTime.UtcNow.AddMinutes(120),
                Issuer = _config["Jwt:Issuer"],
                SigningCredentials = credentials,
                
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

/*             var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, authModel.GUID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, authModel.Email),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }; */

/*             var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials); */

            //    return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }



    }

    // Logout on the Client Side. Remove Token. Doesn't truly elimate the non expired token tho.. ideas? 

}