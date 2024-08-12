using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IzotaDummy.Controllers {

    [ApiController]
    [Route("/api/postgres/")]
    public class PostgresController : ControllerBase
    {
        
        private  string _connectionString = "Host=localhost;Port=5432;Database=izota-test;Username=izota;Password=123qwe";

        public PostgresController()
        {

            string connectionStringENV = Environment.GetEnvironmentVariable("PostgresConnectionString");
            
            _connectionString = String.IsNullOrEmpty(connectionStringENV) ? _connectionString : connectionStringENV;

        }

        [HttpGet("get-data")]
        public async Task<ActionResult<IEnumerable<Testing>>> GetTestings()
        {
            var testings = new List<Testing>();

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT id, note FROM testing", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                testings.Add(new Testing
                {
                    Id = reader.GetInt32(0),
                    Note = reader.GetString(1)
                });
            }

            return Ok(testings);
        }

        [HttpGet("random")]
        public async Task<IActionResult> CreateRandomTesting()
        {
            var random = new Random();
            var note = $"Sample Note {random.Next(1000, 9999)}";
            var testing = new Testing
            {
                Note = note
            };

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("INSERT INTO \"testing\" (\"note\") VALUES (@note) RETURNING \"id\"", conn))
                {
                    cmd.Parameters.AddWithValue("note", testing.Note);
                    testing.Id = (long)await cmd.ExecuteScalarAsync();
                }
            }

            return CreatedAtAction(nameof(GetTesting), new { id = testing.Id }, testing);
        }

        // GET: api/testing/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Testing>> GetTesting(int id)
        {
            Testing? testing = null;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT \"id\", \"note\" FROM \"testing\" WHERE \"id\" = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            testing = new Testing
                            {
                                Id = (int)reader.GetInt64(0),
                                Note = reader.GetString(1)
                            };
                        }
                    }
                }
            }

            if (testing == null)
            {
                return NotFound();
            }

            return testing;
        }


    }

    public class Testing
    {
        public long Id { get; set; }
        public string? Note { get; set; }
    }
}