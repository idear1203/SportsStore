using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void TestCanAddNewLines()
        {
            // Preparation - create test products
            Product p1 = new Product {ProductID = 1, Name = "P1"};
            Product p2 = new Product {ProductID = 2, Name = "P2"};

            // Preparation - Create a new cart
            Cart target = new Cart();

            // Action
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            // Assertions
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void TestCanAddQuantityForExistingLines()
        {
            // Preparation - Create test products
            Product p1 = new Product {ProductID = 1, Name = "P1"};
            Product p2 = new Product {ProductID = 2, Name = "P2"};

            // Preparation - Create a new cart
            Cart target = new Cart();

            // Action
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            // Assertions
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void TestCanRemoveLine()
        {
            // Preparation - Create test products
            Product p1 = new Product {ProductID = 1, Name = "P1"};
            Product p2 = new Product {ProductID = 2, Name = "P2"};
            Product p3 = new Product {ProductID = 3, Name = "P3"};

            // Preparation - Create a new cart
            Cart target = new Cart();

            // Preparation - Add some items
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            // Action
            target.RemoveLine(p2);

            // Assertions
            Assert.AreEqual(target.Lines.Count(c => c.Product == p2), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void TestCalculateCartTotal()
        {
            // Preparation - Create test products
            Product p1 = new Product {ProductID = 1, Name = "P1", Price = 100M};
            Product p2 = new Product {ProductID = 2, Name = "P2", Price = 50M};

            // Preparation - Create a new cart
            Cart target = new Cart();

            // Action
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            // Assertions
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void TestCanClearContents()
        {
            // Preparation - Create test products
            Product p1 = new Product {ProductID = 1, Name = "P1", Price = 100M};
            Product p2 = new Product {ProductID = 2, Name = "P2", Price = 50M};

            // Preparation - Create a new cart
            Cart target = new Cart();

            // Preparation - Add some items
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            // Action - clear cart
            target.Clear();

            // Assertions
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void TestCanAddToCart()
        {
            // Preparation - create mock db
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1", Category = "Apples" }
            }.AsQueryable());

            // Preparation - create cart
            Cart cart = new Cart();

            // Preparation - create controller
            CartController target = new CartController(mock.Object, null);

            // Action - add an item
            target.AddToCart(cart, 1, null);

            // Assertions
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void TestAddingProductToCartGoesToCartScreen()
        {
            // Preparation - create mock db
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1", Category = "Apples" }
            }.AsQueryable());

            // Preparation - create cart
            Cart cart = new Cart();

            // Preparation - create controller
            CartController target = new CartController(mock.Object, null);

            // Action - add an item to cart
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            // Assertions
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void TestCanViewCartContent()
        {
            // Preparation - create cart
            Cart cart = new Cart();

            // Preparation - create controller
            CartController target = new CartController(null, null);

            // Action - call `Index` action method
            CartIndexViewModel result = (CartIndexViewModel) target.Index(cart, "myUrl").ViewData.Model;

            // Assertions
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void TestCannotCheckoutEmptyCart()
        {
            // Preparation - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            // Preparation - create an empty cart
            Cart cart = new Cart();

            // Preparation - create an instance of shipping details
            ShippingDetails shippingDetails = new ShippingDetails();

            // Preparation - create a controller instance
            CartController target = new CartController(null, mock.Object);

            // Action
            ViewResult result = target.Checkout(cart, shippingDetails);

            // Assert - check that order is not passed to processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);

            // Assert - check that view returned by the method is default view
            Assert.AreEqual("", result.ViewName);

            // Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void TestCannotCheckoutInvalidShippingDetails()
        {
            // Preparation - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            // Preparation - create an empty cart
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // Preparation - create a controller instance
            CartController target = new CartController(null, mock.Object);

            // Preparation - add an error to model
            target.ModelState.AddModelError("error", "error");

            // Action - attempt to check out
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // Assert - check that order is not passed to processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);

            // Assert - check that view returned by the method is default view
            Assert.AreEqual("", result.ViewName);

            // Assert - check that I am passing an invalid model to view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void TestCanCheckoutAndSubmitOrder()
        {
            // Preparation - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            // Preparation - create an empty cart
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // Preparation - create a controller instance
            CartController target = new CartController(null, mock.Object);

            // Action - attempt to check out
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // Assert - check that order has been passed to processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once);

            // Assert - check that view returned by the method is "Completed" view
            Assert.AreEqual("Completed", result.ViewName);

            // Assert - check that we are passing an valid model to view
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
