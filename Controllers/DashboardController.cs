using Healthometer_API.Models;
using Healthometer_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Healthometer_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService) =>
        _dashboardService = dashboardService;

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Dashboard>> Get(string id)
    {
        var result = await _dashboardService.GetAsync(id);

        if (result is null)
        {
            return NoContent();
        }

        return result;
    }
}