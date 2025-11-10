using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.BusinessLayer.IService;
using System.Net;

namespace Myto_VignettesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ResponseDetail), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResponseDetail), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null ||
                string.IsNullOrWhiteSpace(loginRequest.Email) ||
                string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest(new ResponseDetail
                {
                    IsError = true,
                    Message = "Email and password are required.",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            try
            {
                var response = await _authService.LoginAsync(loginRequest);
                return StatusCode((int)response.StatusCode!, response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseDetail
                {
                    IsError = true,
                    Message = "An internal server error occurred.",
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorDetail = ex.Message
                });
            }
        }
    }
}
