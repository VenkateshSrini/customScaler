using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using scaler.control.plain.Repo;

namespace scaler.control.plain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlPlaneController : ControllerBase
    {
        private readonly ILogger<ControlPlaneController> _logger;
        private readonly IControlPlanRepo _controlPlaneRepo;
        public ControlPlaneController(IControlPlanRepo controlPlaneRepo, 
            ILogger<ControlPlaneController> logger)
        {
            _controlPlaneRepo = controlPlaneRepo;
            _logger = logger;
        }
    }
}
