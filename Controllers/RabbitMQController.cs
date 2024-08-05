using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IzotaDummy.Controllers {

    [ApiController]
    [Route("api/rabbitmq")]
    public class RabbitMQController : ControllerBase
    {
        private readonly RabbitMQService _rabbitMQService;

        public RabbitMQController(RabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpGet("send")]
        public IActionResult SendMessage([FromQuery] string queue, [FromQuery] string message)
        {
            _rabbitMQService.SendMessage(queue, message);
            return Ok(new { message = "Message sent successfully" });
        }
    }
}