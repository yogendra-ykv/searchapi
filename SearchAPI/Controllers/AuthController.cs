namespace SearchAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SearchAPI.Model;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// The Login
        /// </summary>
        /// <param name="loginRequest">The loginRequest<see cref="LoginRequest"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest.Username == "test" && loginRequest.Password == "password")
            {
                var token = _tokenService.GenerateToken(loginRequest.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }

}
