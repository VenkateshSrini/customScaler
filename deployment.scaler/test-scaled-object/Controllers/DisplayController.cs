using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace test_scaled_object.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisplayController : ControllerBase
    {
        private readonly ILogger<DisplayController> _logger;
        private readonly IConfiguration _config;
        public DisplayController(ILogger<DisplayController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public ActionResult Get() {
            var podName = _config["podName"];
            return Ok($"Hello from {podName}");
        }
    }
}
