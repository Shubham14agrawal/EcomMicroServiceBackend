using System.ComponentModel.DataAnnotations;

namespace Ecom.ProductService
{
        public class Dtos
        {
                public record ItemDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate, string Category, string Subcategory, string Types, string ImageUrl, int Inventory);

                public record CreateItemDto([Required] string Name, string Description, [Range(0, 1000000)] decimal Price, string Category, string Subcategory, string Type, string ImageUrl, int Inventory);

                public record UpdateItemDto([Required] string Name, string Description, [Range(0, 1000000)] decimal Price, string Category, string Subcategory, string Type, string ImageUrl, int Inventory);
                public record CategoryDto(int categoryId, string categoryName);

                public record CreateCategoryDto(string categoryName, string categoryImageUrl);
        }
}
