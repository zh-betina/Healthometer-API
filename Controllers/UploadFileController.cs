using Healthometer_API.Models;
using Microsoft.AspNetCore.Mvc;
using Healthometer_API.Services;

namespace Healthometer_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadFileController : ControllerBase
{
    private readonly FileService _fileService;
    
    public UploadFileController(FileService fileService) =>
        _fileService = fileService;

    [HttpPost("{id:length(24)}")]
    public async Task<IActionResult> Post([FromForm] DocFile file, string id)
    {
        
        var docFile = file.FileContent;
        var userId = id;

        if (docFile == null) throw new Exception("File is null");
        if (userId == null) throw new Exception("User id is missing");

        var response = await _fileService.OnPostUploadAsync(userId, docFile);
        return CreatedAtAction(nameof(Post), new {response});
    }
}