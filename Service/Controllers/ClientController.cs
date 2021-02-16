using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using LeaderboardAPI.Interfaces;

namespace LeaderboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public ClientController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet]
        public async Task<ActionResult<List<LeaderboardRowDTO>>> GetAllRows() 
        {
            var result = await _leaderboardService.Get(null);
            if (result.Count == 0)
            {
                return NoContent();
            }
            return result;
        }
            

        [HttpGet("{clientId}")]
        public async Task<ActionResult<List<LeaderboardRowDTO>>> GetRow([FromRoute] long clientId)
        {
            var result = await _leaderboardService.Get(clientId);

            if (result.Count == 0)
            {
                return NoContent();
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Post(LeaderboardRowDTO row)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _leaderboardService.Get(row.ClientId);

            if (result.Count == 0)
            {
                await _leaderboardService.Create(row);
                return Ok();
            }

            return Conflict(new { message = $"An existing enttry with from client {row.ClientId} was found" });
        }

        [HttpPut]
        public async Task<IActionResult> Update(LeaderboardRowDTO rowIn)
        {
            var result = await _leaderboardService.Get(rowIn.ClientId);

            if (result.Count == 0)
            {
                return NotFound();
            }

            await _leaderboardService.Update(rowIn);
            return Ok();
        }

        [HttpDelete("{clientId}")]
        public async Task<IActionResult> Delete([FromRoute] long clientId)
        {
            var result = await _leaderboardService.Get(clientId);

            if (result.Count == 0)
            {
                return NotFound();
            }

            await _leaderboardService.Delete(clientId);
            return Ok();
        }
    }
}