using CompanyService.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;

namespace CompanyService.Tests.Integration.Middleware
{
    /// <summary>
    /// Tests de integración para ReportAuthorizationMiddleware
    /// </summary>
    public class ReportAuthorizationMiddlewareTests : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly Mock<ILogger<ReportAuthorizationMiddleware>> _loggerMock;

        public ReportAuthorizationMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<ReportAuthorizationMiddleware>>();

            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddSingleton(_loggerMock.Object);
                    });
                    webHost.Configure(app =>
                    {
                        app.UseReportAuthorizationMiddleware();
                        app.Run(async context =>
                        {
                            await context.Response.WriteAsync("Success");
                        });
                    });
                });

            var host = hostBuilder.Start();
            _server = host.GetTestServer();
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Middleware_WithNormalRequest_PassesThrough()
        {
            // Act
            var response = await _client.GetAsync("/test");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("Success", content);
        }

        [Fact]
        public async Task Middleware_WithUnauthorizedAccessException_ReturnsUnauthorized()
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddSingleton(_loggerMock.Object);
                    });
                    webHost.Configure(app =>
                    {
                        app.UseReportAuthorizationMiddleware();
                        app.Run(async context =>
                        {
                            throw new UnauthorizedAccessException("User does not have permission to access this report");
                        });
                    });
                });

            using var host = hostBuilder.Start();
            using var server = host.GetTestServer();
            using var client = server.CreateClient();

            // Act
            var response = await client.GetAsync("/test");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            
            Assert.NotNull(errorResponse);
            Assert.True(errorResponse.ContainsKey("error"));
            Assert.True(errorResponse.ContainsKey("message"));
        }

        [Fact]
        public async Task Middleware_WithPermissionArgumentException_ReturnsForbidden()
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddSingleton(_loggerMock.Object);
                    });
                    webHost.Configure(app =>
                    {
                        app.UseReportAuthorizationMiddleware();
                        app.Run(async context =>
                        {
                            throw new ArgumentException("Insufficient permissions to perform this action", "permission");
                        });
                    });
                });

            using var host = hostBuilder.Start();
            using var server = host.GetTestServer();
            using var client = server.CreateClient();

            // Act
            var response = await client.GetAsync("/test");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            
            Assert.NotNull(errorResponse);
            Assert.True(errorResponse.ContainsKey("error"));
            Assert.True(errorResponse.ContainsKey("message"));
        }

        [Fact]
        public async Task Middleware_WithNonPermissionArgumentException_PassesThrough()
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddSingleton(_loggerMock.Object);
                    });
                    webHost.Configure(app =>
                    {
                        app.UseReportAuthorizationMiddleware();
                        app.Run(async context =>
                        {
                            throw new ArgumentException("Some other argument error", "otherParam");
                        });
                    });
                });

            using var host = hostBuilder.Start();
            using var server = host.GetTestServer();
            using var client = server.CreateClient();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await client.GetAsync("/test");
            });
        }

        [Fact]
        public async Task Middleware_WithOtherException_PassesThrough()
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddSingleton(_loggerMock.Object);
                    });
                    webHost.Configure(app =>
                    {
                        app.UseReportAuthorizationMiddleware();
                        app.Run(async context =>
                        {
                            throw new InvalidOperationException("Some other error");
                        });
                    });
                });

            using var host = hostBuilder.Start();
            using var server = host.GetTestServer();
            using var client = server.CreateClient();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await client.GetAsync("/test");
            });
        }

        [Fact]
        public async Task Middleware_LogsUnauthorizedAccess()
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddSingleton(_loggerMock.Object);
                    });
                    webHost.Configure(app =>
                    {
                        app.UseReportAuthorizationMiddleware();
                        app.Run(async context =>
                        {
                            throw new UnauthorizedAccessException("Test unauthorized access");
                        });
                    });
                });

            using var host = hostBuilder.Start();
            using var server = host.GetTestServer();
            using var client = server.CreateClient();

            // Act
            await client.GetAsync("/test");

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unauthorized access attempt")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Middleware_LogsPermissionDenied()
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddSingleton(_loggerMock.Object);
                    });
                    webHost.Configure(app =>
                    {
                        app.UseReportAuthorizationMiddleware();
                        app.Run(async context =>
                        {
                            throw new ArgumentException("Test permission denied", "permission");
                        });
                    });
                });

            using var host = hostBuilder.Start();
            using var server = host.GetTestServer();
            using var client = server.CreateClient();

            // Act
            await client.GetAsync("/test");

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Permission denied")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Middleware_ReturnsJsonContentType()
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddSingleton(_loggerMock.Object);
                    });
                    webHost.Configure(app =>
                    {
                        app.UseReportAuthorizationMiddleware();
                        app.Run(async context =>
                        {
                            throw new UnauthorizedAccessException("Test error");
                        });
                    });
                });

            using var host = hostBuilder.Start();
            using var server = host.GetTestServer();
            using var client = server.CreateClient();

            // Act
            var response = await client.GetAsync("/test");

            // Assert
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        [InlineData("PATCH")]
        public async Task Middleware_HandlesAllHttpMethods(string httpMethod)
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddSingleton(_loggerMock.Object);
                    });
                    webHost.Configure(app =>
                    {
                        app.UseReportAuthorizationMiddleware();
                        app.Run(async context =>
                        {
                            throw new UnauthorizedAccessException("Test error");
                        });
                    });
                });

            using var host = hostBuilder.Start();
            using var server = host.GetTestServer();
            using var client = server.CreateClient();

            // Act
            var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/test");
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Middleware_WithComplexPath_HandlesCorrectly()
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureServices(services =>
                    {
                        services.AddSingleton(_loggerMock.Object);
                    });
                    webHost.Configure(app =>
                    {
                        app.UseReportAuthorizationMiddleware();
                        app.Run(async context =>
                        {
                            throw new UnauthorizedAccessException("Test error");
                        });
                    });
                });

            using var host = hostBuilder.Start();
            using var server = host.GetTestServer();
            using var client = server.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/reports/123/execute?format=csv");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            
            // Verify logging includes path information
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("/api/v1/reports/123/execute")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}