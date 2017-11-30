using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void TestIndexContainsAllProducts()
        {
            // Preparation - create mock db
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"}
            });

            // Preparation - create controller
            AdminController target = new AdminController(mock.Object);

            // Actions
            Product[] result = ((IEnumerable<Product>) target.Index().ViewData.Model).ToArray();

            // Assertions
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0]);
            Assert.AreEqual("P2", result[1]);
            Assert.AreEqual("P3", result[2]);
        }

        [TestMethod]
        public void TestCanSaveValidChanges()
        {
            // Preparation - create mock DB
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            // Preparation - create controller
            AdminController target = new AdminController(mock.Object);

            // Preparation - create a product
            Product product = new Product { Name = "Test" };

            // Action - attempt to save this product
            ActionResult result = target.Edit(product);

            // Assert - check that calls repository
            mock.Verify(m => m.SaveProduct(product));

            // Assert - check result type -_-
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void TestCannotSaveInvalidChanges()
        {
            // Preparation - create mock DB
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            // Preparation - create controller
            AdminController target = new AdminController(mock.Object);

            // Preparation - create a product
            Product product = new Product { Name = "Test" };

            // Add error to model state
            target.ModelState.AddModelError("error", "error");

            // Action - attempt to save product
            ActionResult result = target.Edit(product);

            // Assert - make sure repository is not called
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never);

            // Assert - check return type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void TestCanDeleteValidProducts()
        {
            // Preparation - create a product
            Product prod = new Product {ProductID = 2, Name = "Test"};

            // Preparation - create mock db
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                prod,
                new Product {ProductID = 3, Name = "P3"},
            });

            // Preparation - create controller
            AdminController target = new AdminController(mock.Object);

            // Action - Delete a product
            target.Delete(prod.ProductID);
        }
    }
}
