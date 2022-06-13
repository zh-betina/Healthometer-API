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

        [HttpPost("{id:length(24)}")]
        public async Task<IActionResult> Post(string id, MedicalVisit newVisit, string? familyMember)
        {
                await _visitsService.PostAsync(id, newVisit, familyMember);
                return CreatedAtAction(nameof(Post), new {newVisit});
        }
}