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

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] DocFile file)
    {
        var docFile = file.FileContent;
        var docInfo = file.Doc;
        var userId = file.UserId;

        if (docFile == null) throw new Exception("File is null");
        if (docFile.Length == 0) throw new Exception("File is empty");
        if (userId == null) throw new Exception("User id is missing");

        await _fileService.OnPostUploadAsync(userId, docFile, docInfo);
        return CreatedAtAction(nameof(Post), new {docFile});
    }
}