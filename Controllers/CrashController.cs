using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IzotaDummy.Controllers
{
    [ApiController]
    [Route("/api/")]
    public class CrashController : ControllerBase
    {
        [HttpGet("crash")]
        public IActionResult CrashMe()
        {
            // throw new Exception("Service crashed intentionally!");
            Environment.Exit(0);

            // Trả về một phản hồi sau khi tắt ứng dụng (không cần thiết vì ứng dụng đã dừng)
            return Ok("Service stopped!");
        }
    }
}