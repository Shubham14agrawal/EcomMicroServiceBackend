namespace Ecom.ProductService.Contracts
{
    public class Contracts
    {
        public record CatalogItemCreated(Guid ItemId, string Name, string Description, decimal Price, int Inventory);
        public record CatalogItemUpdated(Guid ItemId, string Name, string Description, decimal Price, int Inventory);
        public record CatalogItemDeleted(Guid ItemId);

    }
}
