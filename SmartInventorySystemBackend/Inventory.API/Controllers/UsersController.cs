using Inventory.BusinessLogic;
using Inventory.DTOs.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> Get()
        {
            var Users = await clsUsers.GetAllUsers();
            return Ok(Users);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetById(int id)
        {
            var User = await clsUsers.Find(id);
            if (User == null)
            {
                return NotFound($"clsUser with {id} not found.");
            }
            return Ok(User);
        }


        [HttpPost]
        public async Task<ActionResult<UserModel>> Add(UserModel user)
        {
            clsUsers newUser = new clsUsers();
            newUser.Username = user.Username;
            newUser.Password = user.Password;
            newUser.Role = user.Role;


            if (!await newUser.Save())
            {
                return BadRequest("Failed to add new user.");
            }
            else
            {
                user.UserID = newUser.UserID;
                return CreatedAtAction(nameof(GetById), new { id = user.UserID }, user);
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UserModel user)
        {
            if (id != user.UserID)
            {
                return BadRequest("User ID mismatch.");
            }

            clsUsers clsUser = new clsUsers();
            clsUser.UserID = id;
            clsUser.Username = user.Username;
            clsUser.Password = user.Password;
            clsUser.Role = user.Role;
            clsUser.Mode = clsUsers.enMode.update;

            if (!await clsUser.Save())
            {
                return BadRequest("Failed to update user.");
            }
            else
            {
                return NoContent();
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (await clsUsers.Delete(id))
            {
                return NoContent();
            }
            else
            {
                return BadRequest($"Failed to delete user with id {id}.");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDTO loginData)
        {
            try
            {
                var user = await clsUsers.FindByUsernameAndPassword(loginData.Username, loginData.Password);
                if (user == null)
                {
                    return Unauthorized("Invalid username or password.");
                }
                else
                {
                    return Ok(new
                    {
                        Token = "Secret_Token_123",
                        Role = user.Role

                    });


                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}

