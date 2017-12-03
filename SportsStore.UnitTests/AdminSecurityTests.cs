using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void TestCanLoginWithValidCredentials()
        {
            // Prepare - create mock authencation provider
            Mock<IAuthProvier> mock  = new Mock<IAuthProvier>();
            mock.Setup(m => m.Authenticate("admin", "secret")).Returns(true);

            // Prepare - create view model
            LoginViewModel model = new LoginViewModel
            {
                UserName = "admin",
                Password = "secret"
            };

            // Prepare - create controller
            AccountController target = new AccountController(mock.Object);

            // Action - use valid credential
            ActionResult result = target.Login(model, "/MyURL");

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyURL", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void TestCannotLoginWithInvalidCredentials()
        {
            // Prepare - create mock authencation provider
            Mock<IAuthProvier> mock = new Mock<IAuthProvier>();
            mock.Setup(m => m.Authenticate("badUser", "badPass")).Returns(false);

            // Prepare - create view model
            LoginViewModel model = new LoginViewModel
            {
                UserName = "badUser",
                Password = "badPass"
            };

            AccountController target = new AccountController(mock.Object);

            // Action - Use invalid credential
            ActionResult result = target.Login(model, "/MyURL");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
