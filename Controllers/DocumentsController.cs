using Healthometer_API.Models;
using Healthometer_API.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Healthometer_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly DocumentsService _documentsService;

    public DocumentsController(DocumentsService documentsService) =>
        _documentsService = documentsService;

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<List<Document>>> Get(string id, string category = "all")
    {
        var documents = await _documentsService.GetAsync(id, category);

        if (documents is null)
        {

            return NoContent();
        }

        return documents;
    }
    
    [HttpDelete]
    public async Task<CreatedAtActionResult> Delete(string userId, [FromBody] List<string> docsToRemove)
    {
        await _documentsService.DeleteAsync(userId, docsToRemove);
        return CreatedAtAction(nameof(Delete), new {docsToRemove});
    }

    [HttpPatch]
    public async Task<IActionResult> Patch(string userId, string docId, Document modifiedDocument)
    {
        await _documentsService.ModifyAsync(userId, docId, modifiedDocument);
        return Ok();
    }
}