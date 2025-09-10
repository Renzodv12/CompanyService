using CompanyService.Core.Attributes;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;
using Xunit;
using System.Net;

namespace CompanyService.Tests.Unit.Attributes
{
    /// <summary>
    /// Tests unitarios para ReportPermissionAttribute
    /// </summary>
    public class ReportPermissionAttributeTests
    {
        private readonly Mock<IReportAuthorizationService> _authServiceMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<ActionExecutingContext> _actionContextMock;
        private readonly Guid _testUserId = Guid.NewGuid();
        private readonly Guid _testCompanyId = Guid.NewGuid();
        private readonly Guid _testReportId = Guid.NewGuid();

        public ReportPermissionAttributeTests()
        {
            _authServiceMock = new Mock<IReportAuthorizationService>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _httpContextMock = new Mock<HttpContext>();
            _actionContextMock = new Mock<ActionExecutingContext>();

            // Setup service provider
            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IReportAuthorizationService)))
                .Returns(_authServiceMock.Object);

            // Setup HTTP context
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, _testUserId.ToString()),
                new("CompanyId", _testCompanyId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            _httpContextMock.Setup(c => c.User).Returns(principal);
            _httpContextMock.Setup(c => c.RequestServices).Returns(_serviceProviderMock.Object);

            // Setup action context
            var actionContext = new ActionContext
            {
                HttpContext = _httpContextMock.Object
            };

            _actionContextMock.Setup(c => c.HttpContext).Returns(_httpContextMock.Object);
            _actionContextMock.Setup(c => c.ActionArguments).Returns(new Dictionary<string, object>());
        }

        [Fact]
        public async Task OnAuthorizationAsync_WithValidPermission_AllowsAccess()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.ViewReports);
            
            _authServiceMock
                .Setup(s => s.HasPermissionAsync(_testUserId, _testCompanyId, ReportPermission.ViewReports))
                .ReturnsAsync(true);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.Null(_actionContextMock.Object.Result);
            _authServiceMock.Verify(
                s => s.HasPermissionAsync(_testUserId, _testCompanyId, ReportPermission.ViewReports),
                Times.Once);
        }

        [Fact]
        public async Task OnAuthorizationAsync_WithInvalidPermission_DeniesAccess()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.CreateReports);
            
            _authServiceMock
                .Setup(s => s.HasPermissionAsync(_testUserId, _testCompanyId, ReportPermission.CreateReports))
                .ReturnsAsync(false);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.NotNull(_actionContextMock.Object.Result);
            Assert.IsType<ForbidResult>(_actionContextMock.Object.Result);
        }

        [Fact]
        public async Task OnAuthorizationAsync_WithOwnershipRequired_ChecksReportAccess()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.EditOwnReports, requireOwnership: true);
            
            _actionContextMock.Setup(c => c.ActionArguments)
                .Returns(new Dictionary<string, object> { { "reportId", _testReportId } });

            _authServiceMock
                .Setup(s => s.CanAccessReportAsync(_testUserId, _testCompanyId, _testReportId))
                .ReturnsAsync(true);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.Null(_actionContextMock.Object.Result);
            _authServiceMock.Verify(
                s => s.CanAccessReportAsync(_testUserId, _testCompanyId, _testReportId),
                Times.Once);
        }

        [Fact]
        public async Task OnAuthorizationAsync_WithOwnershipRequired_NoReportId_DeniesAccess()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.EditOwnReports, requireOwnership: true);
            
            _actionContextMock.Setup(c => c.ActionArguments)
                .Returns(new Dictionary<string, object>());

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.NotNull(_actionContextMock.Object.Result);
            Assert.IsType<BadRequestObjectResult>(_actionContextMock.Object.Result);
        }

        [Fact]
        public async Task OnAuthorizationAsync_WithOwnershipRequired_InvalidReportAccess_DeniesAccess()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.DeleteOwnReports, requireOwnership: true);
            
            _actionContextMock.Setup(c => c.ActionArguments)
                .Returns(new Dictionary<string, object> { { "reportId", _testReportId } });

            _authServiceMock
                .Setup(s => s.CanAccessReportAsync(_testUserId, _testCompanyId, _testReportId))
                .ReturnsAsync(false);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.NotNull(_actionContextMock.Object.Result);
            Assert.IsType<ForbidResult>(_actionContextMock.Object.Result);
        }

        [Fact]
        public async Task OnAuthorizationAsync_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.ViewReports);
            
            var unauthenticatedPrincipal = new ClaimsPrincipal();
            _httpContextMock.Setup(c => c.User).Returns(unauthenticatedPrincipal);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.NotNull(_actionContextMock.Object.Result);
            Assert.IsType<UnauthorizedResult>(_actionContextMock.Object.Result);
        }

        [Fact]
        public async Task OnAuthorizationAsync_MissingUserId_ReturnsUnauthorized()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.ViewReports);
            
            var claimsWithoutUserId = new List<Claim>
            {
                new("CompanyId", _testCompanyId.ToString())
            };
            var identity = new ClaimsIdentity(claimsWithoutUserId, "Test");
            var principal = new ClaimsPrincipal(identity);
            _httpContextMock.Setup(c => c.User).Returns(principal);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.NotNull(_actionContextMock.Object.Result);
            Assert.IsType<UnauthorizedResult>(_actionContextMock.Object.Result);
        }

        [Fact]
        public async Task OnAuthorizationAsync_MissingCompanyId_ReturnsUnauthorized()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.ViewReports);
            
            var claimsWithoutCompanyId = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, _testUserId.ToString())
            };
            var identity = new ClaimsIdentity(claimsWithoutCompanyId, "Test");
            var principal = new ClaimsPrincipal(identity);
            _httpContextMock.Setup(c => c.User).Returns(principal);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.NotNull(_actionContextMock.Object.Result);
            Assert.IsType<UnauthorizedResult>(_actionContextMock.Object.Result);
        }

        [Fact]
        public async Task OnAuthorizationAsync_ServiceNotAvailable_ReturnsServerError()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.ViewReports);
            
            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IReportAuthorizationService)))
                .Returns((IReportAuthorizationService)null);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.NotNull(_actionContextMock.Object.Result);
            var result = Assert.IsType<ObjectResult>(_actionContextMock.Object.Result);
            Assert.Equal(500, result.StatusCode);
        }

        [Theory]
        [InlineData("id")]
        [InlineData("reportDefinitionId")]
        [InlineData("reportId")]
        public async Task OnAuthorizationAsync_WithDifferentReportIdParameterNames_FindsReportId(string parameterName)
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.EditOwnReports, requireOwnership: true);
            
            _actionContextMock.Setup(c => c.ActionArguments)
                .Returns(new Dictionary<string, object> { { parameterName, _testReportId } });

            _authServiceMock
                .Setup(s => s.CanAccessReportAsync(_testUserId, _testCompanyId, _testReportId))
                .ReturnsAsync(true);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.Null(_actionContextMock.Object.Result);
            _authServiceMock.Verify(
                s => s.CanAccessReportAsync(_testUserId, _testCompanyId, _testReportId),
                Times.Once);
        }

        [Fact]
        public async Task OnAuthorizationAsync_WithStringReportId_ParsesCorrectly()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.EditOwnReports, requireOwnership: true);
            
            _actionContextMock.Setup(c => c.ActionArguments)
                .Returns(new Dictionary<string, object> { { "reportId", _testReportId.ToString() } });

            _authServiceMock
                .Setup(s => s.CanAccessReportAsync(_testUserId, _testCompanyId, _testReportId))
                .ReturnsAsync(true);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.Null(_actionContextMock.Object.Result);
            _authServiceMock.Verify(
                s => s.CanAccessReportAsync(_testUserId, _testCompanyId, _testReportId),
                Times.Once);
        }

        [Fact]
        public async Task OnAuthorizationAsync_WithInvalidGuidFormat_ReturnsBadRequest()
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(ReportPermission.EditOwnReports, requireOwnership: true);
            
            _actionContextMock.Setup(c => c.ActionArguments)
                .Returns(new Dictionary<string, object> { { "reportId", "invalid-guid" } });

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            Assert.NotNull(_actionContextMock.Object.Result);
            Assert.IsType<BadRequestObjectResult>(_actionContextMock.Object.Result);
        }

        [Theory]
        [InlineData(ReportPermission.ViewReports)]
        [InlineData(ReportPermission.CreateReports)]
        [InlineData(ReportPermission.EditOwnReports)]
        [InlineData(ReportPermission.DeleteOwnReports)]
        [InlineData(ReportPermission.ExecuteReports)]
        [InlineData(ReportPermission.ExportReports)]
        [InlineData(ReportPermission.ViewExecutionHistory)]
        [InlineData(ReportPermission.EditAllReports)]
        [InlineData(ReportPermission.DeleteAllReports)]
        [InlineData(ReportPermission.ViewAllExecutions)]
        [InlineData(ReportPermission.ManageReportPermissions)]
        public async Task OnAuthorizationAsync_WithDifferentPermissions_CallsCorrectService(ReportPermission permission)
        {
            // Arrange
            var attribute = new ReportPermissionAttribute(permission);
            
            _authServiceMock
                .Setup(s => s.HasPermissionAsync(_testUserId, _testCompanyId, permission))
                .ReturnsAsync(true);

            // Act
            await attribute.OnAuthorizationAsync(_actionContextMock.Object);

            // Assert
            _authServiceMock.Verify(
                s => s.HasPermissionAsync(_testUserId, _testCompanyId, permission),
                Times.Once);
        }
    }
}