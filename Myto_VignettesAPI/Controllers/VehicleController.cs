using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.BusinessLayer.IService;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using System.Net;

namespace Myto_VignettesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Register a new vehicle for a user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterVehicle([FromBody] VehicleCreateRequest vehicle)
        {
            if (vehicle == null)
            {
                return BadRequest(new ResponseDetail
                {
                    Message = "Vehicle data is required.",
                    StatusCode = HttpStatusCode.BadRequest,
                    IsError = true
                });
            }

            var result = await _vehicleService.CreateAsync(vehicle);
            return StatusCode((int)result.StatusCode!, result);
        }

        /// <summary>
        /// Update existing vehicle details
        /// </summary>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateVehicle(long id, [FromBody] VehicleCreateRequest vehicle)
        {
            if (vehicle == null)
            {
                return BadRequest(new ResponseDetail
                {
                    Message = "Vehicle data is required.",
                    StatusCode = HttpStatusCode.BadRequest,
                    IsError = true
                });
            }

            vehicle.Id = id;
            var result = await _vehicleService.UpdateAsync(vehicle);
            return StatusCode((int)result.StatusCode!, result);
        }

        /// <summary>
        /// Delete vehicle by Id
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVehicle(long id)
        {
            if (id <= 0)
            {
                return BadRequest(new ResponseDetail
                {
                    Message = "Invalid vehicle ID.",
                    StatusCode = HttpStatusCode.BadRequest,
                    IsError = true
                });
            }

            var result = await _vehicleService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode!, result);
        }

        /// <summary>
        /// Get details of a specific vehicle by ID
        /// </summary>
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetVehicleById(long id)
        {
            if (id <= 0)
            {
                return BadRequest(new ResponseDetail
                {
                    Message = "Invalid vehicle ID.",
                    StatusCode = HttpStatusCode.BadRequest,
                    IsError = true
                });
            }

            var result = await _vehicleService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode!, result);
        }

        /// <summary>
        /// Get all vehicles for a specific user
        /// </summary>
        [HttpGet("GetAllList")]
        public async Task<IActionResult> GetVehiclesByUser(int pageIndex = 0, int pageSize = 20)
        {

            var result = await _vehicleService.GetAllByUserAsync(pageIndex, pageSize);
            return StatusCode((int)result.StatusCode!, result);
        }

        // 2️⃣ Get vehicle details by registration number
        [HttpGet("registration/{registrationNumber}")]
        public async Task<IActionResult> GetVehicleByRegistration(string registrationNumber)
        {
            var result = await _vehicleService.GetByRegistrationNumberAsync(registrationNumber);
            return StatusCode((int)result.StatusCode!, result);
        }
    }
}
