using System.Net.Http.Headers;
using Healthometer_API.Models;
using Microsoft.AspNetCore.Mvc;
using Healthometer_API.Services;
using Microsoft.OpenApi.Any;
using MongoDB.Bson;

namespace Healthometer_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadFileController : ControllerBase
{
    private readonly UploadFileService _uploadFileService;
    
    public UploadFileController(UploadFileService uploadFileService) =>
        _uploadFileService = uploadFileService;

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] DocFile docFile)
    {
        //THIS IS WORKING, now refactor
        // if (docFile.FileContent == null) throw new Exception("File is null");
        // if (docFile.FileContent.Length == 0) throw new Exception("File is empty");
        //
        // var uploadedFile = Request.Form.Files[0];
        // var userId = docFile.UserId;
        // Console.WriteLine(userId);
        // _uploadFileService.CreateDirectoryForUser(userId);
        //
        // var randomName = Path.GetRandomFileName();
        // var folderName = Path.Combine("Docs", userId, randomName);
        // var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //
        // if (uploadedFile.Length > 0)
        // {
        //     await using (var stream = new FileStream(pathToSave, FileMode.Create))
        //         docFile.FileContent.CopyTo(stream);
        //
        //     return Ok(new {pathToSave});
        // }
        // return BadRequest("Not ok");
        
        await _uploadFileService.OnPostUploadAsync(docFile);
        return CreatedAtAction(nameof(Post), new {docFile});
    }
}