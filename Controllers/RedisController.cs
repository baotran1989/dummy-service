using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace IzotaDummy.Controllers {
    [ApiController]
    [Route("api/redis")]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _database;

        public RedisController()
        {
        
            var redisConnectionString = "localhost:6379,password=redispassword";
            string connectionStringENV = Environment.GetEnvironmentVariable("AppSettings__RedisConnectionString");
            string connectionString = String.IsNullOrEmpty(connectionStringENV) ? redisConnectionString : connectionStringENV;
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _database = redis.GetDatabase();
        }

        [HttpGet("set")]
        public async Task<IActionResult> SetData([FromQuery] string key, [FromQuery] string value)
        {
            await _database.StringSetAsync(key, value);
            return Ok(new { message = "Data set successfully" });
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetData([FromQuery] string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return NotFound(new { message = "Data not found" });
            }
            return Ok(new { key = key, value = value.ToString() });
        }
    }

}