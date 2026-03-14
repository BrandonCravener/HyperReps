using System.Security.Claims;
using HyperReps.Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace HyperReps.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMessageBus _bus;

        public UsersController(IMessageBus bus)
        {
            _bus = bus;
        }

        [HttpGet("me/spotify-token")]
        public async Task<ActionResult<SpotifyTokenResponse>> GetMySpotifyToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var tokenData = await _bus.InvokeAsync<SpotifyTokenResponse?>(
                new GetSpotifyTokenQuery(userId)
            );

            if (tokenData == null)
            {
                return StatusCode(403, new { message = "Re-authenticate with Spotify." });
            }

            return Ok(tokenData);
        }
    }
}
