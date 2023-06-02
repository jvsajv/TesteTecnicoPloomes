using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Scrypt;
using TesteTecnicoPloomes.Data;
using TesteTecnicoPloomes.DTO;
using TesteTecnicoPloomes.Enums;
using TesteTecnicoPloomes.Models;
using TesteTecnicoPloomes.Repositories;
using TesteTecnicoPloomes.Repositories.Interfaces;
using TesteTecnicoPloomes.Services;

namespace TesteTecnicoPloomes.Controllers
{

    [Route("v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> AuthenticateAsync([FromBody] UserDTO model)
        {
            ScryptEncoder encoder = new ScryptEncoder();
            
            var authenticated = (from c
                                     in await _userRepository.GetAllAsync()
                                 where c.Username.Equals(model.Username)
                                 select c).SingleOrDefault();

            if (authenticated != null)
            {
                var isValidUser = encoder.Compare(model.Password, authenticated.Password);
                if (isValidUser)
                {
                    var token = TokenService.GenerateToken(authenticated);
                    authenticated.Password = "";

                    return Ok(new {
                        user = authenticated.Username,
                        token = token
                    });
                }
            }

            return BadRequest(new { message = "Wrong Username/Password!" });
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<dynamic>> RegisterAsync([FromBody] UserDTO model)
        {
            ScryptEncoder encoder = new ScryptEncoder();
            DatabaseContext db = new DatabaseContext();

            var user = (from c
                                     in await _userRepository.GetAllAsync()
                                 where c.Username.Equals(model.Username)
                                 select c).SingleOrDefault();


            if (user != null)
            {
                return BadRequest(new { message = "Username already in use." });
            }
            else
            {
                User newUser = new User();
                newUser.Username = model.Username;
                newUser.Password = encoder.Encode(model.Password);

                try
                {
                    newUser.Role = RoleUser.Viewer;
                    await _userRepository.Add(newUser);

                    return Ok(new { message = "Registered Succefully" });
                } catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
