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
    public async Task<ActionResult<List<Document>>> Get(string id)
    {
        var documents = await _documentsService.GetAsync(id);

        if (documents is null)
        {

            return NoContent();
        }

        return documents;
    }

    [HttpPut]
    public async Task<IActionResult> Put(string id, Document newDocument)
    {
        await _documentsService.PutAsync(id, newDocument);
        return CreatedAtAction(nameof(Put), new {newDocument});
    }

}