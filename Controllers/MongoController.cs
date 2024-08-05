using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace IzotaDummy.Controllers {
    [Route("/api/mongo/")]
    [ApiController]
    public class MongoController : ControllerBase
    {
        private readonly IMongoCollection<UserInformation> _users;

        public MongoController()
        {
            string connectionStringENV = Environment.GetEnvironmentVariable("AppSettings__MongoConnectionString");
            string connectionString = String.IsNullOrEmpty(connectionStringENV) ? "mongodb://izota:123qwe@localhost:27017" : connectionStringENV;
   
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("izota_test");
            _users = database.GetCollection<UserInformation>("user_information");
        }

        [HttpGet("get-data")]
        public async Task<ActionResult<IEnumerable<Testing>>> GetTestings()
        {
            var testings = await _users.Find(new BsonDocument()).ToListAsync();
            return Ok(testings);
        }

        [HttpGet("create-dummy")]
        public async Task<IActionResult> CreateRandomUser()
        {
            // Tạo dữ liệu ngẫu nhiên cho trường name
            var random = new Random();
            var user = new UserInformation
            {
                Name = $"User_{random.Next(1000, 9999)}"
            };

            await _users.InsertOneAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserInformation>> GetUser(string id)
        {
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }

    public class UserInformation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
    }
}