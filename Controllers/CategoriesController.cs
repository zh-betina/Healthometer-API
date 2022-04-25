using Healthometer_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Healthometer_API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class CategoriesController : ControllerBase
{
    private readonly CategoriesService _categoriesService;

    public CategoriesController(CategoriesService categoriesService) =>
        _categoriesService = categoriesService;

    [HttpGet("{id:length(24)}")]

    public async Task<ActionResult<List<string>>> GetAsync(string id)
    {
        var categories = await _categoriesService.GetAsync(id);

        if (categories.Count == 0)
        {
            return NoContent();
        }

        return categories;
    }

    [HttpPost]
    public async Task<IActionResult> Post(string userId, string newCategory)
    {
        await _categoriesService.PostAsync(userId, newCategory);
        return CreatedAtAction(nameof(Post), new {newCategory});
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string userId, string category)
    {
        await _categoriesService.DeleteAsync(userId, category);
        return CreatedAtAction(nameof(Delete), new {category});
    }

    [HttpPatch]
    public async Task<IActionResult> Patch(string userId, string oldCategory, string newCategory)
    {
        await _categoriesService.ModifyAsync(userId, oldCategory, newCategory);
        return CreatedAtAction(nameof(Patch), new {newCategory});
    }
}