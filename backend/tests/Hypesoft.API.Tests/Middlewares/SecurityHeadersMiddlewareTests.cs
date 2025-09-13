using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Hypesoft.API.Tests.Middlewares
{
    public class SecurityHeadersMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public SecurityHeadersMiddlewareTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SecurityHeaders_ShouldBePresent_OnAllResponses()
        {
            var client = _factory.CreateClient();

            // Make a GET request to any endpoint, e.g., /health
            var response = await client.GetAsync("/health");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.Headers.Should().ContainKey("X-Content-Type-Options");
            response.Headers.Should().ContainKey("X-Frame-Options");
            response.Headers.Should().ContainKey("X-XSS-Protection");
            response.Headers.Should().ContainKey("Referrer-Policy");
            response.Headers.Should().ContainKey("Content-Security-Policy");
        }
    }
}
