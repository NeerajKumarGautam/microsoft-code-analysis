﻿using ECommerce.API.Model;
using ECommerse.ProductCatalog.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {

        private static readonly string Product_Details = "";
        private static readonly string Customer_Details = "";

        private readonly IProductCatalogService _service;
        public ProductsController()
        {
            var proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            _service = proxyFactory.CreateServiceProxy<IProductCatalogService>(
                new Uri("fabric:/Ecomm/ECommerce.ProductCatalog"),
                new ServicePartitionKey(0));
        }

        [HttpGet]
        public async Task<IEnumerable<ApiProduct>> GetAsync()
        {
            IEnumerable<Product> allProducts = await _service.GetAllProductsAsync();
            IEnumerable<ApiProduct> product = null;
            try
            {
                product = allProducts.Select(p => new ApiProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    IsAvailable = p.Availability
                });
            }
            catch (Exception e)
            {
                // Console.WriteLine(e);

            }
            return product;
        }

        [HttpPost]
        public async Task PostAsync([FromBody] ApiProduct product)
        {

            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Availability = 100
            };
            await _service.AddProductAsync(newProduct);
        }

    }
}
