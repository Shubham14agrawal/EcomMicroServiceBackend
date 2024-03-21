using Ecom.Cart.Contracts;
using Ecom.Common;
using Ecom.ProductService.Entities;
using MassTransit;
using static Ecom.ProductService.Contracts.Contracts;
using static Ecom.ProductService.Dtos;

namespace Ecom.ProductService.Consumers
{
    public class OrderPlaceItemsConsumer: IConsumer<PlaceOrderItems>
    {
        private readonly IRepository<Item> repository;
        private readonly IPublishEndpoint publishEndpoint;

        public OrderPlaceItemsConsumer(IRepository<Item> repository, IPublishEndpoint publishEndpoint)
        {
            this.repository = repository;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<PlaceOrderItems> context)
        {
            var message = context.Message;

            foreach (var orderItem in message.cartItems)
            {
                var productId = orderItem.CatalogItemId;
                var buyQuantity = orderItem.Quantity;
                var availableQuantity = (await repository.GetAsync(item => item.Id == productId)).Inventory;
                if (availableQuantity < buyQuantity)
                {
                    throw new InvalidOperationException("The stock is less than required.");
                }
            }

            foreach (var orderItem in message.cartItems)
            {
                var productId = orderItem.CatalogItemId;
                var buyQuantity = orderItem.Quantity;
                var catalogItem = await repository.GetAsync(item => item.Id == productId);
                await repository.UpdateAsync(new Item
                {
                    Id = catalogItem.Id,
                    Name = catalogItem.Name,
                    Description = catalogItem.Description,
                    Price = catalogItem.Price,
                    Category = catalogItem.Category,
                    Subcategory = catalogItem.Subcategory,
                    Type = catalogItem.Type,
                    ImageUrl = catalogItem.ImageUrl,
                    Inventory = catalogItem.Inventory - orderItem.Quantity
                });

                await publishEndpoint.Publish(new CatalogItemUpdated(catalogItem.Id, catalogItem.Name, catalogItem.Description , catalogItem.Price , catalogItem.Inventory - orderItem.Quantity));
            }
        }
    }
    
}
