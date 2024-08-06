using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace IzotaDummy.Controllers
{
    [ApiController]
    [Route("/api/cpu/")]
    public class BurnCPUController : ControllerBase
    {
        
        private bool burningCPU = false;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private Task burningTask;

        [HttpGet("burn")]
        public IActionResult StartCPUBurn()
        {
            // if (!burningCPU)
            // {
            //     burningCPU = true;
            //     burningTask = Task.Run(() => BurnCPU());
            // }

            if (burningTask == null || burningTask.IsCompleted)
            {
                cancellationTokenSource = new CancellationTokenSource();
                burningTask = Task.Run(() => BurnCPU(cancellationTokenSource.Token), cancellationTokenSource.Token);
            }

            return GetCPUInfo();
        }

        [HttpGet("stop-burn")]
        public IActionResult StopCPUBurn()
        {
            // cancellationTokenSource?.Cancel();
            // if (burningCPU)
            // {
            //     burningCPU = false;
            //     burningTask.Wait();
            // }
            if (burningTask != null && !burningTask.IsCompleted)
            {
                cancellationTokenSource.Cancel();
                burningTask.Wait(); // Chờ tác vụ hoàn thành
            }

            return GetCPUInfo();
        }

        [HttpGet("info")]
        public IActionResult GetCPUInfo()
        {
            int numCPU = Environment.ProcessorCount;
            // float cpuUsage = 0f;

            //  using (Process process = Process.GetCurrentProcess())
            //  {
            //      cpuUsage = (float)(process.TotalProcessorTime.TotalMilliseconds / (Environment.TickCount64 * Environment.ProcessorCount)) * 100;
            //  }

            return Ok(new
            {
                data = new
                {
                    num_cpu = numCPU,
                    cpu_usage = ReadCpuUsage()
                }
            });
        }

        private void BurnCPU(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int result = 0;
                for (int i = 0; i < 1000000; i++)
                {
                    result += i;
                }
            }
            // // Đoạn mã sau sẽ chạy trên mỗi lõi CPU và đốt CPU
            // Parallel.For(0, Environment.ProcessorCount, (i, loopState) =>
            // {
            //     while (!cancellationToken.IsCancellationRequested)
            //     {
            //         // Đoạn mã này được thực hiện để đốt CPU
            //     }
            // });
        }


        public double ReadCpuUsage()
        {
            var output = "";
            var cpuInfo = new ProcessStartInfo("top -b -n 1");
            cpuInfo.FileName = "/bin/bash";
            cpuInfo.Arguments = "-c \"top -b -n 1\"";
            cpuInfo.RedirectStandardOutput = true;

            using (var process = Process.Start(cpuInfo))
            {
                output = process.StandardOutput.ReadToEnd();
                //Console.WriteLine(output);
            }

            var cpuLine = output.Split("\n").ToList();
            var cpuLine2 = cpuLine[2].Split(",", StringSplitOptions.RemoveEmptyEntries);
            var firstPart = cpuLine2[0].Split(":", StringSplitOptions.RemoveEmptyEntries);
            var secondPart = cpuLine2[1].Split("s", StringSplitOptions.RemoveEmptyEntries);
            var thirdPart = cpuLine2[2].Split("n", StringSplitOptions.RemoveEmptyEntries);

            double cpuUsage = double.Parse(firstPart[1].Split("u", StringSplitOptions.RemoveEmptyEntries)[0]) +
                    double.Parse(secondPart[0]) +
                    double.Parse(thirdPart[0]);

            cpuUsage = Math.Round(cpuUsage, 2);

            return cpuUsage;
        }


        private float CalculateCPUUsage()
        {
            float cpuUsage = 0f;

            using (Process process = Process.GetCurrentProcess())
            {
                TimeSpan totalProcessorTime = process.TotalProcessorTime;
                TimeSpan systemUptime = TimeSpan.FromSeconds(Environment.TickCount / 1000);
                int numProcessors = Environment.ProcessorCount;

                // Tính toán thời gian CPU đã sử dụng tính bằng phần trăm
                if (systemUptime.TotalSeconds > 0 && numProcessors > 0)
                {
                    double cpuUsageInTicks = totalProcessorTime.Ticks / (systemUptime.Ticks * numProcessors);
                    cpuUsage = (float)(cpuUsageInTicks * 100);
                }
            }

            return cpuUsage;
        }
    }
}
