using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IzotaDummy.Services
{
    public class MemoryMetrics
    {
        public double Total;
        public double Used;
        public double Free;
    }
 
    public class MemoryMetricsClient
    {
        public MemoryMetrics GetMetrics()
        {
            MemoryMetrics metrics;
 
            if (IsUnix())
            {
                metrics = GetUnixMetrics();
            }
            else
            {
                metrics = GetWindowsMetrics();
            }
  
            return metrics;
        }
 
        private bool IsUnix()
        {
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
 
            return isUnix;
        }
 
        private MemoryMetrics GetWindowsMetrics()
        {
            var output = "";
 
            var info = new ProcessStartInfo();
            info.FileName = "wmic";
            info.Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value";
            info.RedirectStandardOutput = true;
 
            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }
 
            var lines = output.Trim().Split("\n");
            var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);
 
            var metrics = new MemoryMetrics();
            metrics.Total = Math.Round(double.Parse(totalMemoryParts[1]), 0);
            metrics.Free = Math.Round(double.Parse(freeMemoryParts[1]), 0);
            metrics.Used = metrics.Total - metrics.Free;
 
            return metrics;
        }
 
        // private MemoryMetrics GetUnixMetrics()
        // {
        //     var output = "";

        //     var info = new ProcessStartInfo("free");
        //     info.Arguments = "-m";
        //     info.RedirectStandardOutput = true;

        //     using (var process = Process.Start(info))
        //     {
        //         output = process.StandardOutput.ReadToEnd();
        //         Console.WriteLine(output);
        //     }

        //     var lines = output.Split("\n");
        //     if (lines.Length < 2)
        //     {
        //         throw new Exception("No data returned from 'free' command");
        //     }

        //     var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        //     if (memory.Length < 4)
        //     {
        //         throw new Exception("Invalid data format returned from 'free' command");
        //     }

        //     var metrics = new MemoryMetrics();
        //     metrics.Total = double.Parse(memory[1]);
        //     metrics.Used = double.Parse(memory[2]);
        //     metrics.Free = double.Parse(memory[3]);

        //     return metrics;
        // }

        private MemoryMetrics GetUnixMetrics()
        {
            var metrics = new MemoryMetrics();

            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"cat /proc/meminfo\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            })
            {
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Xử lý đầu ra từ tệp /proc/meminfo để lấy thông tin bộ nhớ
                var lines = output.Split('\n');
                foreach (var line in lines)
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim().Split(' ')[0]; // Lấy phần số từ dòng

                        switch (key)
                        {
                            case "MemTotal":
                                metrics.Total = double.Parse(value) / 1024; // Đổi KB thành MB
                                break;
                            case "MemFree":
                                metrics.Free = double.Parse(value) / 1024; // Đổi KB thành MB
                                break;
                            case "MemAvailable":
                                metrics.Used = (metrics.Total - double.Parse(value)) / 1024; // Đổi KB thành MB
                                break;
                        }
                    }
                }
            }

            return metrics;
        }
    }
}