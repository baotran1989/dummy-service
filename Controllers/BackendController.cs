using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace IzotaDummy.Controllers
{
    [ApiController]
    [Route("/api/backend/")]
    public class BackendController : ControllerBase
    {

        [HttpGet("message-a")]
        public async Task<IActionResult> GetMessageA()
        {
            return Ok(new
            {
                status = 200,
                message = "Message from backend endpoint A"
            });
        }

        [HttpGet("message-b")]
        public async Task<IActionResult> GetMessageB()
        {
            return Ok(new
            {
                status = 200,
                message = "Message from backend endpoint B"
            });
        }

        [HttpGet("message-c")]
        public async Task<IActionResult> GetMessageC()
        {
            return Ok(new
            {
                status = 200,
                message = "Message from backend endpoint C"
            });
        }
    }

    
}
