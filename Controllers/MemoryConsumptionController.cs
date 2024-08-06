using Microsoft.AspNetCore.Mvc;
using IzotaDummy.Services;
using System;

namespace IzotaDummy.Controllers
{
    [ApiController]
    [Route("/api/memory")]
    public class MemoryConsumptionController : ControllerBase
    {
        // Biến global để lưu trữ memory allocation
        private static long memoryAllocated = 0;
        private static List<byte[]> dataAllocatedMemory = new List<byte[]>();

        [HttpGet("allocate")]
        public IActionResult AllocateMemory()
        {
            // Allocate thêm 16Mb memory và cập nhật biến global
            AllocateMemory(16);

            return ResponseMemory();
        }

        public IActionResult ResponseMemory() {
            
            var client = new MemoryMetricsClient();
            var metrics = client.GetMetrics();
            var freeMemory = metrics.Free;
            var total = metrics.Total;

            var responseData = new
            {
                status = 200, // HTTP status code
                message = $"Allocated 16Mb memory. Total memory allocated: {memoryAllocated} Mb", // Thông điệp
                data = new {
                    free = freeMemory,
                    total = total
                } 
            };
            return Ok(responseData);
        }

        [HttpGet("deallocate")]
        public IActionResult CleanMemory()
        {
            // Deallocate toàn bộ memory đã allocate trước đó và cập nhật biến global
           DeallocateMemory();
           return ResponseMemory();
        }


        // Phương thức để allocate/deallocate memory và cập nhật biến global
        private static void AllocateMemory(int sizeMb)
        {
            byte[] memoryChunk = new byte[sizeMb * 1024 * 1024];

            for (int i = 0; i < memoryChunk.Length; i++)
            {
                memoryChunk[i] = 1; // Ghi dữ liệu vào từng byte của mảng
            }

            dataAllocatedMemory.Add(memoryChunk);
            memoryAllocated =+ sizeMb;

            // Đảm bảo biến global không bị GC
            GC.KeepAlive(dataAllocatedMemory);
        }

        private static void DeallocateMemory()
        {
            memoryAllocated = 0;
            dataAllocatedMemory.Clear();
        }
    }
}
