using Healthometer_API.Models;
using Healthometer_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Healthometer_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicalVisitController : ControllerBase
{
        private readonly MedicalVisitsService _visitsService;

        public MedicalVisitController(MedicalVisitsService medicalVisitsService) =>
                _visitsService = medicalVisitsService;

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<List<MedicalVisit>>> Get(string id, string? familyMemberId, bool? isRegular, bool? isDone)
        {
                var visits = await _visitsService.GetAsync(id, familyMemberId, isRegular, isDone);

                if (visits is null)
                {
                        return NoContent();
                }

                return visits;
        }

        [HttpPost("{id:length(24)}")]
        public async Task<IActionResult> Post(string id, MedicalVisit newVisit, string? familyMember)
        {
                await _visitsService.PostAsync(id, newVisit, familyMember);
                return CreatedAtAction(nameof(Post), new {newVisit});
        }

        [HttpPatch]
        public async Task<IActionResult> Patch(string userId, string medicalVisitId)
        {
                await _visitsService.PatchAsync(userId, medicalVisitId);
                return CreatedAtAction(nameof(Patch), new { });
        }
}