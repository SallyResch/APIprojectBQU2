using APIprojectBQU.Data;
using APIprojectBQU.Helpers;
using APIprojectBQU.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace APIprojectBQU.Controllers
{
    [Route("/api/controller")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly APIprojectDbContext aPIprojectDbContext;
        public UserController(APIprojectDbContext _APIprojectDbContext)
        {
            aPIprojectDbContext = _APIprojectDbContext;
        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await aPIprojectDbContext.Users.AnyAsync(u => u.Email == email);
        }

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if(password.Length <8) 
                sb.Append("Minimum password length should be 8 " + Environment.NewLine);
            
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be Alphanumeric " + Environment.NewLine);

            if (!Regex.IsMatch(password, "[!]"))
                sb.Append("Password should contain special character " + Environment.NewLine);
            return sb.ToString();
        }

        private string CreateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.ASCII.GetBytes("1my3secret6key9/lökajsdopjmkdfglkmcvinowemr023948587");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email)
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = credentials
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        } 
 
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObject)
        {
            if (userObject == null)
                return BadRequest();


            //Check if Email is unique
            if (await CheckEmailExistAsync(userObject.Email))
            {
                return BadRequest (new { message = "User Email already exists " + userObject.Email });
            }


            //Check the password Strength
            var pswd=CheckPasswordStrength(userObject.Password);
            if(!string.IsNullOrEmpty(pswd))
                return BadRequest(new {message = pswd.ToString()});

            userObject.Password = PasswordHashing.HashPassword(userObject.Password);
            userObject.Role = "User";
            userObject.Token = "Make Token with jwt";
            await aPIprojectDbContext.Users.AddAsync(userObject);
            await aPIprojectDbContext.SaveChangesAsync();
                return Ok(new 
                { 
                    Message = "User registered" 
                });
            
        }


        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObject)
        {
            if (userObject == null)
            
                return BadRequest();

            var user = await aPIprojectDbContext.Users.FirstOrDefaultAsync(x => x.Email == userObject.Email);
            if (user == null)
                return NotFound(new { Message = "user not found" });

            if(!PasswordHashing.VerifyPassword(userObject.Password, user.Password))
            {
                return BadRequest(new { Message = "password is incorrect" });
            }
            user.Token = CreateJwtToken(user);

            return Ok(new 
            { 
                Token = user.Token,
                Message = "Login Success!" 
            });
        }
    }
}
