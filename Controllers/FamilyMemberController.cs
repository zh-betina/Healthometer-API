using Healthometer_API.Models;
using Healthometer_API.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Healthometer_API.Controllers;

[ApiController]
[Route("api/[controller]/")]

public class FamilyMemberController : ControllerBase
{
    private readonly FamilyMemberService _familyMemberService;

    public FamilyMemberController(FamilyMemberService familyMemberService) =>
        _familyMemberService = familyMemberService;

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<List<FamilyMember>>> Get(string id)
    {
        var familyMembers = await _familyMemberService.GetAsync(id);

        if (familyMembers is null)
        {
            return NoContent();
        }

        return familyMembers;
    }
    
    [HttpGet("Documents/{userId:length(24)}&{familyId:length(24)}")]
    public async Task<List<Document>> GetDocs(string userId, string familyId)
    {
        var familyMembers = await _familyMemberService.GetDocsAsync(userId, familyId);
    
        if (familyMembers is null)
        {
            return new List<Document>();
        }
    
        return familyMembers;
    }

    [HttpPost]
    public async Task<string> Post(string userId, FamilyMember newFamilyMember)
    {
        var update = await _familyMemberService.PostAsync(userId, newFamilyMember);

        if (update == "ok")
        {
            return "Ok, it's done";
        }

        return "Not ok";
    }

    [HttpDelete]
    public async Task<string> Delete(string userId, string familyMemberId)
    {
        var removal = await _familyMemberService.DeleteAsync(userId, familyMemberId);

        if (removal == "ok")
        {
            return "Ok, deleted";
        }

        return "Not deleted";
    }
}