using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Models;
using CreativeHubWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CreativeHubWebApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly ResourceService _resources;

        public UsersController(AuthService auth, ResourceService resources)
        {
            _auth = auth;
            _resources = resources;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue("sub")!;

        [Authorize]
        [HttpGet("me")]  
        public async Task<IActionResult> GetMe()
        {
            var profile = await _auth.GetProfileAsync(UserId);
            return profile is null ? NotFound() : Ok(profile);
        }

        [Authorize]
        [HttpPut("me")]   
        public async Task<IActionResult> UpdateMe(UpdateProfileDto dto)
        {
            var updated = await _auth.UpdateProfileAsync(UserId, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [Authorize]
        [HttpGet("me/resources")]   
        public async Task<IActionResult> MyResources()
        {
            var list = await _resources.GetByOwnerAsync(UserId);
            return Ok(list);
        }

        [Authorize]
        [HttpDelete("me")]   
        public async Task<IActionResult> DeleteMe()
        {
            var ok = await _auth.DeleteAccountAsync(UserId);
            return ok ? NoContent() : NotFound();
        }
    }
}