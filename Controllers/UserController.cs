using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scrypt;
using TesteTecnicoPloomes.Data;
using TesteTecnicoPloomes.Enums;
using TesteTecnicoPloomes.Models;
using TesteTecnicoPloomes.Repositories.Interfaces;

namespace TesteTecnicoPloomes.Controllers
{
    [Route("v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("fetchAll")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<dynamic>> FetchAllUsers()
        {
            try
            {
                return Ok(new { users = await _userRepository.GetAllAsync() });
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<dynamic>> FetchUserById(int id)
        {
            try
            {
                return Ok(await _userRepository.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("resetPassword")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<dynamic>> ResetUserPassword([FromBody] string username)
        {
            ScryptEncoder encoder = new ScryptEncoder();

            DatabaseContext db = new DatabaseContext();

            var user = (from c
                                     in await _userRepository.GetAllAsync()
                        where c.Username.Equals(username)
                        select c).SingleOrDefault();


            if (user != null)
            {
                return BadRequest(new { message = "Unable to find user "+ username });
            }
            else
            {
               
                var password = encoder.Encode(username);

                try
                {
                    user.Password = password;
                    await _userRepository.Update(user);

                    await db.SaveChangesAsync();

                    return Ok(new { message = "Password Updated Succefully" });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("updateRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<dynamic>> UpdateUserRole([FromBody] string username, RoleUser role)
        {

            DatabaseContext db = new DatabaseContext();

            var user = (from c
                                     in await _userRepository.GetAllAsync()
                        where c.Username.Equals(username)
                        select c).SingleOrDefault();


            if (user != null)
            {
                return BadRequest(new { message = "Unable to find user " + username });
            }
            else
            {

                

                try
                {
                    user.Role = role;
                    await _userRepository.Update(user);

                    await db.SaveChangesAsync();

                    return Ok(new { message = "Role Updated Succefully" });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}
