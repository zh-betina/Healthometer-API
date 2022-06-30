using Healthometer_API.Models;
using Microsoft.AspNetCore.Mvc;
using Healthometer_API.Services;
using MongoDB.Bson;

namespace Healthometer_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadFileController : ControllerBase
{
    private readonly FileService _fileService;
    private readonly DocumentsService _documentsService;

    public UploadFileController(FileService fileService, DocumentsService docsService)
    {
        _fileService = fileService;
        _documentsService = docsService;
    }
    [HttpPost("{id:length(24)}")]
    public async Task<string> Post([FromForm] DocFile fileForm, string id)
    {
        Console.WriteLine("upload file reached");
        var docFile = fileForm.FileContent;
        var userId = id;

        if (docFile == null) throw new Exception("File is null");
        if (userId == null) throw new Exception("User id is missing");

        var uploadResponse = await _fileService.OnPostUploadAsync(userId, fileForm);
        var infoResponse = await _documentsService.PostAsync(uploadResponse, userId);
        Console.WriteLine(infoResponse.ToJson());
        return "ok";
    }
}