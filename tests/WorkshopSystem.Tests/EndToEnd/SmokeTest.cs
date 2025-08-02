using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using WorkshopSystem.Core.Application.DTOs;

namespace WorkshopSystem.Tests.EndToEnd
{
    public class SmokeTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SmokeTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task HomePage_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsSuccess()
        {
            var login = new { Email = "admin@carservice.com", Password = "Dorset001^" };
            var response = await _client.PostAsJsonAsync("/Account/Login", login);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Api_ServiceRecords_ReturnsData()
        {
            await LoginAsAdminAsync();
            var response = await _client.GetAsync("/api/ServiceRecords");
            response.EnsureSuccessStatusCode();
        }

        private async Task LoginAsAdminAsync()
        {
            var login = new { Email = "admin@carservice.com", Password = "Dorset001^" };
            await _client.PostAsJsonAsync("/Account/Login", login);
        }
    }
}
