using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using scaler.control.plain.Model;
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
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DeploymentScaleInfo request)
        {
            var response = await _controlPlaneRepo.UpsertDeploymentInfoAsync(request);
            return Ok(response);
        }
        [HttpGet("{ns}/{name}")]
        public async Task<IActionResult> Get([FromRoute] string ns, [FromRoute] string name)
        {
            var response = await _controlPlaneRepo.GetDeploymentScaleInfoAsync(ns, name);
            return Ok(response);
        }
        [HttpGet("metrics/{ns}/{name}")]
        public async Task<IActionResult> GetMetrics([FromRoute] string ns, [FromRoute] string name)
        {
            var deployment = await _controlPlaneRepo.GetDeploymentScaleInfoAsync(ns, name);
            if (deployment == null)
            {
                _logger.LogCritical($"ScaledObject {name} with namespace {ns} not foud in DB");

                deployment = new DeploymentScaleInfo
                {
                    Name = name,
                    Namespace = ns,
                    IsScalingActive = false,
                    MaxScale = 0,
                    MinScale = 0,

                };
            }
            else
            {
                int desiredScale;
                if (deployment.IsScalingActive)
                {
                    desiredScale = deployment.MaxScale;
                }
                else
                {
                    desiredScale = deployment.MinScale;
                }
                deployment.MaxScale = desiredScale;
            }
            return Ok(deployment);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] string ns, [FromRoute] string name)
        {
            var response = await _controlPlaneRepo.DeleteDeploymentInfoAsync(ns, name);
            return Ok(response);
        }
    }
}
