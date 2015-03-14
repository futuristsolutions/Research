﻿using System.Collections.Generic;

namespace WebApi2.Owin.DomainModel
{
    public class ProductsRepository : IProductsRepository
    {
        #region IProductsRepository Members

        public IList<Product> GetAllProducts()
        {
            return ProductsDataStore.Products;
        }

        public void Insert(Product product)
        {
            ProductsDataStore.Insert(product);
        }

        public void Update(Product product)
        {
            ProductsDataStore.Update(product);
        }

        public void Delete(int productId)
        {
            ProductsDataStore.Delete(productId);
        }

        #endregion
    }
}