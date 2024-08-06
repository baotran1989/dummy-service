using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace IzotaDummy.Controllers
{
    [ApiController]
    [Route("api/memory")]
    public class MemoryController : ControllerBase
    {
        private static List<byte[]> allocatedMemory = new List<byte[]>();

        [HttpGet("allocate")]
        public IActionResult AllocateMemory()
        {
            // Allocate 16MB of memory
            byte[] memoryChunk = new byte[16 * 1024 * 1024];
            allocatedMemory.Add(memoryChunk);

            // Calculate total allocated memory
            long totalAllocatedMemory = allocatedMemory.Count * memoryChunk.Length;

            return Ok(new
            {
                status = 200,
                data = new
                {
                    free = GC.GetTotalMemory(false),
                    total = totalAllocatedMemory
                }
            });
        }

        [HttpGet("clear")]
        public IActionResult ClearMemory()
        {
            // Clear all allocated memory
            allocatedMemory.Clear();

            return Ok(new
            {
                status = 200,
                data = new
                {
                    free = GC.GetTotalMemory(false),
                    total = 0
                }
            });
        }
    }
}
