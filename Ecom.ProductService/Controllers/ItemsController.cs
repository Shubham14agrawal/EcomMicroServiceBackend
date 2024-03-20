using Ecom.Common;
using Ecom.ProductService.Entities;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics.Contracts;
using static Ecom.ProductService.Contracts.Contracts;
using static Ecom.ProductService.Dtos;

namespace Ecom.ProductService.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        private readonly IRepository<Category> categoriesRepository;
        private readonly IPublishEndpoint publishEndpoint;
        public ItemsController(IRepository<Item> itemsRepository, IRepository<Category> categoriesRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
            this.categoriesRepository = categoriesRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync([FromQuery(Name ="category")] string category="all")
        {
            if (category != "all")
            {
                var items = (await itemsRepository.GetAllAsync(item => item.Category == category))
                        .Select(item => item.AsDto());
                return items;

            }
            else
            {
                var items = (await itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());
                return items;
            }
        }

        [HttpGet( "{id}" )]
        public async Task<ActionResult<ItemDto>> GetByIdAsync( Guid id )
        {
            var item = await itemsRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            string imageUrl = createItemDto.ImageUrl;
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow,
                Category = createItemDto.Category,
                Subcategory = createItemDto.Subcategory,
                Type = createItemDto.Type,
                ImageUrl = imageUrl 
            };
            {
                // check if category exist
                var categoryExist = await categoriesRepository.GetAsync(category => category.CategoryName == item.Category);
                if (categoryExist == null)
                {
                    return BadRequest(new
                    {
                        error = "Invalid Category"
                    });
                }
            }
            await itemsRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name,item.Description,item.Price));

            return CreatedAtAction(nameof(GetByIdAsync), new {id = item.Id}, item);  
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto) { 
            var existingItem = await itemsRepository.GetAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;
            existingItem.Category = updateItemDto.Category;
            existingItem.Subcategory = updateItemDto.Subcategory;
            existingItem.Type = updateItemDto.Type;
            existingItem.ImageUrl = updateItemDto.ImageUrl;

            await itemsRepository.UpdateAsync(existingItem);

            await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description, existingItem.Price));

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            await itemsRepository.RemoveAsync(item.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }

        [HttpGet("/categories")]
        public async Task<ActionResult<Category[]>> GetCategories()
        {
            return Ok(await categoriesRepository.GetAllAsync());
        }

        [HttpPost("/categories")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddCategory(CreateCategoryDto category)
        {
            var newCategory = new Category
            {
                CategoryImageUrl = category.categoryImageUrl,
                CategoryName = category.categoryName
            };
            await categoriesRepository.CreateAsync(newCategory);
            return CreatedAtAction(nameof(AddCategory), new { id = newCategory.Id }, newCategory);
        }

    }
}
