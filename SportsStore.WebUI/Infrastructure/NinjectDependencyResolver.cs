using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        enum DbMode
        {
            Mock,
            LocalDb
        }

        private DbMode dbMode = DbMode.LocalDb;

        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            if (dbMode == DbMode.Mock)
            {
                BindToMockDb();
            }
            else if (dbMode == DbMode.LocalDb)
            {
                BindToLocalDb();
            }
        }

        private void BindToMockDb()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product { Name = "Football", Price = 25 },
                new Product { Name = "Surf board", Price = 179 },
                new Product { Name = "Running shoes", Price = 95 }
            });
            kernel.Bind<IProductRepository>().ToConstant(mock.Object);
        }

        private void BindToLocalDb()
        {
            kernel.Bind<IProductRepository>().To<EFProductRepository>();
        }
    }
}