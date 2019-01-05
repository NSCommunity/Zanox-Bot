using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Http;

namespace ZanoxDiscordBot
{
    class APIController : ApiController
    {
        APIItems[] products = new APIItems[]
        {
            new APIItems { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new APIItems { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new APIItems { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };

        public IEnumerable<APIItems> GetAllProducts()
        {
            return products;
        }

        public APIItems GetProductById(int id)
        {
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return product;
        }

        public IEnumerable<APIItems> GetProductsByCategory(string category)
        {
            return products.Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
        }
    }
}
