using Healthometer_API.Models;
using Healthometer_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Healthometer_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly DocumentsService _documentsService;

    public DocumentsController(DocumentsService documentsService) =>
        _documentsService = documentsService;

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<List<Document>>> Get(string id)
    {
        var documents = await _documentsService.GetAsync(id);

        if (documents is null)
        {

            return NoContent();
        }

        return documents;
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete(string userId, string docId)
    {
        await _documentsService.DeleteAsync(userId, docId);
        return CreatedAtAction(nameof(Delete), new {docId});
    }

    [HttpPatch]
    public async Task<IActionResult> Patch(string userId, string docId, Document modifiedDocument)
    {
        await _documentsService.ModifyAsync(userId, docId, modifiedDocument);
        return Ok();
    }
}