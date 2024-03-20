using Ecom.Common;

namespace Ecom.ProductService.Entities
{
    public class Category : IEntity
    {
        public Guid Id { get; set; }

        public string CategoryName { get; set; }

        public string CategoryImageUrl { get; set; }
    }
}
