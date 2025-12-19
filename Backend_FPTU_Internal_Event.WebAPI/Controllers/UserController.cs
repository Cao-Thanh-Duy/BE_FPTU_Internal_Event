using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend_FPTU_Internal_Event.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get All User",
            Description = ""
        )]
        public IActionResult GetAllUser()
        {
            try
            {
                var result = _userService.GetAllUser();
                return Ok(new
                {
                    success = true,
                    message = "Users retrieved successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

        
        //[Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get User by Id",
            Description = ""
        )]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var result = _userService.GetUserById(id);
                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"User with ID {id} not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "User retrieved successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

        
        [Authorize(Roles = "Admin")] 
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create User (Admin)",
            Description = ""
        )]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid input",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var result = _userService.CreateUser(request);

                if (result == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email already exists or RoleId is invalid"
                    });
                }

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = result.UserId },
                    new
                    {
                        success = true,
                        message = "User created successfully",
                        data = result
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut]
        [SwaggerOperation(
            Summary = "Update User",
            Description = ""
        )]
        public IActionResult UpdateUser([FromQuery] int userId, [FromBody] string userName)
        {
            try
            {
                var result = _userService.UpdateUserName(userId, userName);
                if (result != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Update user successfully ",
                        data = result
                    });
                    
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Internal server"
                        

                    });
                }
                
            }
            catch (KeyNotFoundException knx)
            {
                return BadRequest(new
                {
                    success = false,
                    message = knx.Message

                });
            }
            catch(Exception ex) 
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message

                });
            }

        }

        [HttpDelete]
        [SwaggerOperation(
            Summary = "Delete User",
            Description = ""
        )]

        public IActionResult DeleteUser([FromQuery] int userId)
        {
            try
            {
                var result = _userService.DeleteUser(userId);
                if (result != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Delete user successfully ",
                        data = result
                    });

                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Internal server"


                    });
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new 
                {
                    success = false,
                    message = ex.Message

                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-profile-by-admin")]
        [SwaggerOperation(
    Summary = "Update User Profile by Admin",
    Description = "Admin can update Username, Email, Password, and Role of any user. Password is optional - only updated if provided."
)]
        public IActionResult UpdateUserProfileByAdmin([FromQuery] int userId, [FromBody] UpdateUserProfileRequest request)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid input",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var result = _userService.UpdateUserProfileByAdmin(userId, request);

                if (result == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to update user profile"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "User profile updated successfully by Admin",
                    data = result
                });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new
                {
                    success = false,
                    message = knfEx.Message
                });
            }
            catch (InvalidOperationException ioEx)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ioEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

    }
}
