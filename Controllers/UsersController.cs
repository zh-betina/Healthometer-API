using Healthometer_API.Models;
using Healthometer_API.Services;
using Microsoft.AspNetCore.Mvc;


namespace Healthometer_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService) =>
        _usersService = usersService;

    [HttpGet]
    public async Task<List<User>> Get() =>
        await _usersService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> Get(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return user;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(User newUser)
    {
        //TODO add checking if the user with the email exists already
        try
        {
            var requestResult = await _usersService.CreateAsync(newUser);
            if (requestResult == 1)
            {
               return new OkObjectResult(new { status = 201, msg = "The user was created" });
            }

            return new BadRequestObjectResult(new {status = 400, msg = "One of the values is missing"});
        }
        catch (Exception e)
        {
            return new JsonResult(new {status = 500, msg = "DB operation has failed. Try again"});
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        await _usersService.RemoveAsync(id);
        return NoContent();
    }
}