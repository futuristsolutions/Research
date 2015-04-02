using System.Collections.Generic;

namespace WebApi2.Owin.DomainModel
{
    public interface IProductsRepository
    {
        IList<Product> GetAllProducts();

        void Insert(Product product);

        void Update(Product product);

        void Delete(int productId);
    }
}