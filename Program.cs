using System;
using System.Diagnostics;
using IzotaDummy.Configurations;
using IzotaDummy.Services;

Timer? timer = null;

int secondInit = int.TryParse(Environment.GetEnvironmentVariable("AppSettings__StartupTime"), out int result) ? result : 25;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddEnvironmentVariables();


builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddControllers();



builder.Services.AddSingleton<RabbitMQService>(sp =>
{
    // var config = sp.GetRequiredService<IConfiguration>().GetSection("RabbitMQ");
    var hostName =  Environment.GetEnvironmentVariable("AppSettings__MQHost") ?? "localhost";//config["HostName"];
    var userName = Environment.GetEnvironmentVariable("AppSettings__MQUserName") ?? "izota";//config["UserName"];
    var password = Environment.GetEnvironmentVariable("AppSettings__MQPassword") ?? "123qwe";//config["Password"];
    return new RabbitMQService(hostName, userName, password);
});

builder.Services.AddHostedService<RabbitMQBackgroundService>();




var app = builder.Build();
// var delayInSeconds = 20; // Khởi tạo giá trị delay mặc định là 20 giây

var environmentName = Environment.GetEnvironmentVariable("AllowedHosts");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapControllers();

// PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available Bytes");
// long totalMemoryInBytes = (long)ramCounter.NextValue();

// // Chuyển đổi sang đơn vị KB, MB hoặc GB nếu cần
// double totalMemoryInKB = totalMemoryInBytes / 1024.0;
// double totalMemoryInMB = totalMemoryInBytes / (1024.0 * 1024.0);
// double totalMemoryInGB = totalMemoryInBytes / (1024.0 * 1024.0 * 1024.0);

// Console.WriteLine($"Total system memory: {totalMemoryInMB} MB");
// app.UseEndpoints(endpoints =>
// {
//     // Map các endpoint của controller mới vào ứng dụng
//     endpoints.MapControllers();
// });




var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5000).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

timer = new Timer(state =>{
    Counter.Instance.Increment();
    if (Counter.Instance.GetValue() == secondInit) {
        timer!.Dispose();
    }
}, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1)); // Giảm delay mỗi 1 giây sau khi chạy service trong 10 giây


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
