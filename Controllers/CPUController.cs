using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IzotaDummy.Controllers
{
    [ApiController]
    [Route("api/cpu2")]
    public class CPUController : ControllerBase
    {
        private static bool isBurningCPU = false;
        private static readonly object lockObject = new object();

        [HttpGet("burn")]
        public IActionResult BurnCPU()
        {
            lock (lockObject)
            {
                if (!isBurningCPU)
                {
                    isBurningCPU = true;
                    Task.Run(() => BurnCPULoop());
                }
            }

            return Ok(new
            {
                status = 200,
                message = "CPU burning started."
            });
        }

        [HttpGet("stop")]
        public IActionResult StopBurningCPU()
        {
            lock (lockObject)
            {
                isBurningCPU = false;
            }

            return Ok(new
            {
                status = 200,
                message = "CPU burning stopped."
            });
        }

        [HttpGet("info")]
        public IActionResult GetCPUInfo()
        {
            var numCores = Environment.ProcessorCount;
            var cpuUsage = GetTotalCPUUsage();

            return Ok(new
            {
                status = 200,
                data = new
                {
                    num_cpu = numCores,
                    cpu_usage = cpuUsage
                }
            });
        }

        private void BurnCPULoop()
        {
            while (isBurningCPU)
            {
                // Simulate CPU load by performing some calculations
                for (int i = 0; i < 1000000; i++)
                {
                    Math.Sqrt(i);
                }
            }
        }

        private float GetTotalCPUUsage()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
            Task.Delay(500).Wait();
            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;

            var cpuUsedMs = endCpuUsage - startCpuUsage;
            var totalMs = (endTime - startTime).TotalMilliseconds;

            return (float)(cpuUsedMs / totalMs * 100);
        }
    }
}
