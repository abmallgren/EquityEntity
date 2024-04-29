using Microsoft.AspNetCore.Mvc;

namespace Nexus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NexusController : ControllerBase
    {
        private readonly ILogger<NexusController> _logger;

        public NexusController(ILogger<NexusController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "DetermineNexusStatus")]
        public Output Get([FromBody] Input input)
        {
            var output = new Output();

            return output;
        }
    }
}