using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreativeHubWebApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]   
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        public AuthController(AuthService auth) => _auth = auth;

        [HttpPost("register")]    
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var result = await _auth.RegisterAsync(dto);
                return Ok(result);          
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });  
            }
        }

        [HttpPost("login")]      
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _auth.LoginAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message }); 
            }
        }
    }
}