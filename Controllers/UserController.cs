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
    [Route("user")]
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
        public async Task<ActionResult<dynamic>> FetchAllUsers(int skip=0, int take=25)
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
        [Route("{idUser}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<dynamic>> FetchUserById(int idUser)
        {
            try
            {
                return Ok(await _userRepository.GetByIdAsync(idUser));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("{idUser}/resetPassword")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<dynamic>> ResetUserPassword(int idUser)
        {
            ScryptEncoder encoder = new ScryptEncoder();

            DatabaseContext db = new DatabaseContext();

            var user = await _userRepository.GetByIdAsync(idUser);


            if (user == null)
            {
                return BadRequest(new { message = "Unable to find user" });
            }
            else
            {
               
                var password = encoder.Encode(user.Username);

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
        [Route("{idUser}/updateToRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<dynamic>> UpdateUserRole(int idUser, RoleUser role)
        {

            DatabaseContext db = new DatabaseContext();

            var user = await _userRepository.GetByIdAsync(idUser);


            if (user == null)
            {
                return BadRequest(new { message = "Unable to find user"});
            }
            else
            {

                try
                {
                    user.Role = role;
                    await _userRepository.Update(user);

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
