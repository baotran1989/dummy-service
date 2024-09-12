using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using IzotaDummy.Services;

namespace IzotaDummy.Controllers
{
    [ApiController]
    [Route("/api/")]
    public class HealthCheckController : ControllerBase
    {
        readonly int secondInit = int.TryParse(Environment.GetEnvironmentVariable("StartupTime"), out int result) ? result : 25;



        [HttpGet("healthz")]
        public IActionResult Healthz()
        {
            // Counter.Instance.Increment();
            var responseData = new
            {
                status = 200, // HTTP status code
                message = "Service is healthy", // Thông điệp
                data = new { 
                    count = Counter.Instance.GetValue()
                } // Dữ liệu cụ thể, nếu có
            };

            return Ok(responseData);
        }

        [HttpGet("ready")]
        public IActionResult Ready()
        {
            var countTime = Counter.Instance.GetValue();
            var responseData = new
            {
                status = 200, // HTTP status code
                message = "Service is ready", // Thông điệp
                data = new { 
                    countTime = countTime
                }
            };

            if(countTime < secondInit) {
                responseData = new {
                    status = 503, 
                    message = "Service Unavailable",
                    data = new {
                        countTime = countTime
                    }
                };

                return new ObjectResult(responseData){
                    StatusCode = 503
                };
            }

            return Ok(responseData);
        }

    }
}