using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Myto_VignettesAPI.BusinessLayer.IService;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using System.Net;

namespace Myto_VignettesAPI.Controllers
{
    /// <summary>
    /// Handles all user-related operations such as CRUD, validation, and listing.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // ------------------------------------------------------------
        // GET: api/users/{id}
        // ------------------------------------------------------------
        /// <summary>
        /// Retrieves a user by their unique ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var response = await _userService.GetByIdAsync(id);
            return StatusCode((int)response.StatusCode!, response);
        }

        // ------------------------------------------------------------
        // GET: api/users
        // ------------------------------------------------------------
        /// <summary>
        /// Retrieves a paginated list of users.
        /// </summary>
        /// <param name="pageIndex">The page index (0-based).</param>
        /// <param name="pageSize">The number of users per page. If 0, returns all.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllAsync([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 20)
        {
            var response = await _userService.GetAllAsync(pageIndex, pageSize);
            return StatusCode((int)response.StatusCode!, response);
        }

        // ------------------------------------------------------------
        // POST: api/users
        // ------------------------------------------------------------
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user object to create.</param>
        [HttpPost("User_Registration")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User object cannot be null.");

            var response = await _userService.CreateAsync(user);
            return StatusCode((int)response.StatusCode!, response);
        }

        // ------------------------------------------------------------
        // PUT: api/users/{id}
        // ------------------------------------------------------------
        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The user ID to update.</param>
        /// <param name="user">The updated user details.</param>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] User user)
        {
            if (user == null)
                return BadRequest("User object cannot be null.");

            user.Id = id;
            var response = await _userService.UpdateAsync(user);
            return StatusCode((int)response.StatusCode!, response);
        }

        // ------------------------------------------------------------
        // DELETE: api/users/{id}
        // ------------------------------------------------------------
        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        [HttpDelete("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var response = await _userService.DeleteAsync(id);
            return StatusCode((int)response.StatusCode!, response);
        }

        // ------------------------------------------------------------
        // GET: api/users/check-email
        // ------------------------------------------------------------
        /// <summary>
        /// Checks whether a given email is already registered.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        [HttpGet("check-email")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckEmailAsync([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var response = await _userService.ExistsByEmailAsync(email);
            return StatusCode((int)response.StatusCode!, response);
        }
    }
}
