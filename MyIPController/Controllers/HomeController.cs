using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;

namespace IpAddress.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
       
        private readonly HttpClient _httpClient;

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string visitor_name)
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(clientIp))
            {
                return BadRequest("Unable to determine client IP address.");
            }

            var location = await GetLocationFromIp(clientIp);
            var temperature = 11;
            var greeting = $"Hello, {visitor_name}!, the temperature is {temperature} degrees Celsius in {location}";

            var response = new
            {
                client_ip = clientIp,
                location = location,
                greeting = greeting
            };

            return Ok(response);
        }

        private async Task<string> GetLocationFromIp(string ip)
        {
            var response = await _httpClient.GetStringAsync($"https://ipinfo.io/{ip}/json?token=<YOUR_API_KEY>");
            var json = JObject.Parse(response);
            return json["city"]?.ToString() ?? "Unknown";
        }
    }
}
       