using System.Collections.Generic;
using System.Web.Http;
using WebApi2.Owin.DomainModel;

namespace WebApi2.Owin.Api
{
    public class ProductsController : ApiController
    {
        private readonly IProductsRepository _productsRepository = new ProductsRepository();

        public IEnumerable<Product> Get()
        {
            return this._productsRepository.GetAllProducts();
        }
    }
}
