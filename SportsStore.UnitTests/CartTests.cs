using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;

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
            Assert.AreEqual(results[0].Quality, 11);
            Assert.AreEqual(results[1].Quality, 1);
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
    }
}
