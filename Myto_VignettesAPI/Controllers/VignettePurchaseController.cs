using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.BusinessLayer.IService;

namespace Myto_VignettesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VignettePurchaseController : ControllerBase
    {
        private readonly IVignettePurchaseService _service;

        public VignettePurchaseController(IVignettePurchaseService service)
        {
            _service = service;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Create(VignettePurchaseCreateRequest request)
        {
            var result = await _service.CreateAsync(request);
            return StatusCode((int)result.StatusCode!, result);
        }

        [HttpGet("{PaymentId}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode!, result);
        }

        [HttpGet("userpurchasehistory")]
        public async Task<IActionResult> GetByUser(long userId, int pageIndex = 0, int pageSize = 20)
        {
            var result = await _service.GetByUserAsync(userId, pageIndex, pageSize);
            return StatusCode((int)result.StatusCode!, result);
        }

        [HttpPut("payment")]
        public async Task<IActionResult> UpdatePayment(VignettePaymentUpdateRequest request)
        {
            var result = await _service.UpdatePaymentAsync(request);
            return StatusCode((int)result.StatusCode!, result);
        }
    }
}