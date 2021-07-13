using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using user.Models;
using user.Services;

namespace user.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        readonly IUserService service;
        readonly IConfiguration _configuration;
        readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(IUserService userService, IConfiguration configuration, ILogger<AuthenticationController> logger)
        {
            service = userService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("v1/users/authenticate")]
        public IActionResult Post([FromBody] UserDetails user)
        {
            try
            {
                _logger.LogInformation($"Verifying user details and generate token");
                var _user=  service.Login(user);
                return Ok(GetJWTToken(user.Email,_user.UserName));
            }
            catch (UserNotFoundException unf)
            {
                _logger.LogInformation($"Provided credentials are not correct");
                return NotFound(unf.Message);
            }
            catch (Exception)
            {
                _logger.LogError("Error in verifying the user");
                return StatusCode(500);
            }
        }

        [HttpPut("v1/users")]
        [Authorize(AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Put([FromBody] UserDetails user)
        {
            try
            {
                _logger.LogInformation($"Registering a new user");
                service.ChangePassword(user);
                return Ok("Password updated successfully");
            }
            catch (UserNotFoundException unf)
            {
                _logger.LogInformation("User details trying to register already exists");
                return NotFound(unf.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in registering the user {ex.Message}");
                return StatusCode(500);
            }
        }
        [HttpPost("v1/users/register")]
        [ActionName("Post")]
        public IActionResult Post_Register([FromBody] UserDetails user)
        {
            try
            {
                _logger.LogInformation($"Adding a new Customer {user}");
                service.AddUser(user);
                return Created("", "User is registered successfully");
            }
            catch (UserAlreadyExistsException usr)
            {
                _logger.LogInformation($"This customer already exists {user.Email}");
                return Conflict(usr.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Adding the user: {ex.Message}");
                return StatusCode(500);
            }
        }
        private string GetJWTToken(string userId,string userName)
        {
            //setting the claims for the user credential name
            var claims = new[]
           {
                new Claim("userId", userId),
            };

            //Defining security key and encoding the claim 

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            //defining the JWT token essential information and setting its expiration time
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );
            //defing the response of the token 
            var response = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                userName=userName
            };
            //convert into the json by serialing the response object
            return JsonConvert.SerializeObject(response);
        }
    }
}
