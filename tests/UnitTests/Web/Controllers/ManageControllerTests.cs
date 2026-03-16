using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web.Controllers;
using Microsoft.eShopWeb.Web.Services;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Microsoft.eShopWeb.UnitTests.Web.Controllers;
public class ManageControllerTests
{
    [Fact]
    public async Task SendVerificationEmail_sends_email()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            UserName = "test"
        };

        var userManager = TestUserManager(user);
        var signInManager = TestSignInManager(userManager);

        var emailSender = new Mock<IEmailSender>();
        emailSender.Setup(e => e.SendEmailAsync(
                                    user.Email,
                                    It.IsAny<string>(),
                                    It.IsAny<string>()))
                   .Returns(Task.CompletedTask);

        var logger = new Mock<IAppLogger<ManageController>>();
        var urlEncoder = UrlEncoder.Default;

        var controller = new TestableManageController(
                             userManager,
                             signInManager,
                             emailSender.Object,
                             logger.Object,
                             urlEncoder);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) }))
            }
        };

        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                 .Returns("https://localhost/confirm-email");

        controller.Url = urlHelper.Object;

        // Act
        await controller.InvokeSendVerificationEmail(user);

        // Assert
        emailSender.Verify(e => e.SendEmailAsync(
                                    user.Email,
                                    It.IsAny<string>(),
                                    It.IsAny<string>()),
                                    Times.Once);
    }

    #region Helpers
    private static UserManager<ApplicationUser> TestUserManager(ApplicationUser user)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        var manager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        manager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
               .ReturnsAsync(user);

        manager.Setup(m => m.GenerateEmailConfirmationTokenAsync(user))
               .ReturnsAsync("token");

        return manager.Object;
    }

    private static SignInManager<ApplicationUser> TestSignInManager(
        UserManager<ApplicationUser> userManager)
    {
        return new SignInManager<ApplicationUser>(
            userManager,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            null,
            null,
            null,
            null);
    }
    #endregion


    internal class TestableManageController : ManageController
    {
        public TestableManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IAppLogger<ManageController> logger,
            UrlEncoder urlEncoder)
            : base(userManager, signInManager, emailSender, logger, urlEncoder)
        {
        }

        public Task InvokeSendVerificationEmail(ApplicationUser user) => SendVerificationEmailInternalAsync(user);
    }
}
