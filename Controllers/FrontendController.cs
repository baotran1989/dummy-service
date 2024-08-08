using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json.Linq;
using IzotaDummy.Configurations;
using Microsoft.Extensions.Options;

namespace IzotaDummy.Controllers
{
    [ApiController]
    [Route("/api/frontend/")]
    public class FrontendController : ControllerBase
    {
        
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;

        public FrontendController(IOptions<AppSettings> appSettings)
        {
            _httpClient = new HttpClient();
            _appSettings = appSettings.Value;

        }

        [HttpGet("get-message-a")]
        public async Task<IActionResult> GetMessageA()
        {
            
            string apiEndpoint = Environment.GetEnvironmentVariable("ExternalAPIMessageA");
            string externalApiUrl = String.IsNullOrEmpty(apiEndpoint) ? _appSettings.ExternalAPIMessageA : apiEndpoint;
            return await GetMessage(externalApiUrl);
        }

        [HttpGet("get-message-b")]
        public async Task<IActionResult> GetMessageB()
        {
            // string externalApiUrl = _appSettings.ExternalAPIMessageB;
            string apiEndpoint = Environment.GetEnvironmentVariable("ExternalAPIMessageB");
            string externalApiUrl = String.IsNullOrEmpty(apiEndpoint) ? _appSettings.ExternalAPIMessageB : apiEndpoint;
            return await GetMessage(externalApiUrl);
        }

        [HttpGet("get-message-c")]
        public async Task<IActionResult> GetMessageC()
        {
            // string externalApiUrl = _appSettings.ExternalAPIMessageC;
            string apiEndpoint = Environment.GetEnvironmentVariable("ExternalAPIMessageC");
            string externalApiUrl = String.IsNullOrEmpty(apiEndpoint) ? _appSettings.ExternalAPIMessageC : apiEndpoint;
            return await GetMessage(externalApiUrl);
        }

        public async Task<IActionResult> GetMessage(String externalApiUrl)
        {
           
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(externalApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error calling external API");
                }

                string responseData = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseData);

                // Giả sử dữ liệu trả về có dạng { "data": { "message": "your message" } }
                string message = json?["message"]?.ToString();

                if (message == null)
                {
                    return BadRequest("Message not found in the response");
                }

                return Ok(new { message });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Request error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
    }
}
