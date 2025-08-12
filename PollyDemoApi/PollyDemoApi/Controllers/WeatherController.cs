using Microsoft.AspNetCore.Mvc;
using Polly.Wrap;
using PollyDemoApi.Services;

namespace PollyDemoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly FakeUnreliableService _service;
        private readonly AsyncPolicyWrap<string> _policyWrap;

        public WeatherController(FakeUnreliableService service, AsyncPolicyWrap<string> policyWrap)
        {
            _service = service;
            _policyWrap = policyWrap;
        }

        [HttpGet("getdata")]
        public async Task<IActionResult> GetData()
        {
            var result = await _policyWrap.ExecuteAsync(() => _service.GetDataAsync());
            return Content(result, "application/json");
        }
    }
}