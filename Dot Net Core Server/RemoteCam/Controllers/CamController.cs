using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RemoteCam.DTO;
using RemoteCam.Pi;
using System.IO;

namespace RemoteCam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CamController : ControllerBase
    {
        ILogger<CamController> log;

        public CamController(ILogger<CamController> log)
        {
            this.log = log;
        }

        [HttpGet]
        public ActionResult<Capture> Get()
        {
            log.LogInformation("Request for capture");
            return Camera.GetLatestCapture();
        }
    }
}
