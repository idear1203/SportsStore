using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EFProductRepository : IProductRepository
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Product> Products => context.Products;

        public void SaveProduct(Product product)
        {
            if (product.ProductID == 0)
            {
                context.Products.Add(product);
            }
            else
            {
                Product dbEntity = context.Products.Find(product.ProductID);
                if (dbEntity != null)
                {
                    dbEntity.Name = product.Name;
                    dbEntity.Description = product.Description;
                    dbEntity.Price = product.Price;
                    dbEntity.Category = product.Category;
                }
            }

            context.SaveChanges();
        }

        public Product DeleteProduct(int productId)
        {
            Product dbEntity = context.Products.Find(productId);
            if (dbEntity != null)
            {
                context.Products.Remove(dbEntity);
                context.SaveChanges();
            }

            return dbEntity;
        }
    }
}
