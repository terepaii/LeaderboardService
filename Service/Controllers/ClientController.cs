using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

using LeaderboardAPI.Interfaces;

namespace LeaderboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public ClientController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet]
        [Route("{clientId}/{leaderboardId}")]
        public async Task<ActionResult<LeaderboardRowDTO>> Get([FromRoute] Guid clientId, [FromRoute] short leaderboardId)
        {
            var result = await _leaderboardService.Get(clientId, leaderboardId);

            if (result == null)
            {
                Log.Debug($"No rows returned for {Utility.GetFunctionName()}");
                return NoContent();
            }

            return result;
        }

        [HttpGet]
        [Route("{leaderboardId}")]
        public async Task<ActionResult<List<LeaderboardRowDTO>>> GetRowsPaginated([FromRoute] short leaderboardId, [FromQuery] int offset=0, [FromQuery] int limit=10) 
        {
            var result = await _leaderboardService.GetRowsPaginated(leaderboardId, offset, limit);
            if (result.Count == 0)
            {
                Log.Debug($"No rows returned for {Utility.GetFunctionName()}");
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

            // TODO Move to service layer
            var result = await _leaderboardService.Get(row.ClientId, row.LeaderboardId);

            if (result == null)
            {
                await _leaderboardService.Create(row);
                return Ok();
            }

            return Conflict(new { message = $"An existing entry with from client [{row.ClientId}] on leaderboard [{row.LeaderboardId}] was found" });
        }

        [HttpPut]
        public async Task<IActionResult> Update(LeaderboardRowDTO row)
        {
            // TODO Move to service layer
            var result = await _leaderboardService.Get(row.ClientId, row.LeaderboardId);

            if (result == null)
            {
                return NotFound();
            }

            await _leaderboardService.Update(row);
            return Ok();
        }

        [HttpDelete("{clientId}/{leaderboardId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid clientId, short leaderboardId)
        {
            // TODO Move to service layer
            var result = await _leaderboardService.Get(clientId, leaderboardId);

            if (result == null)
            {
                return NotFound();
            }

            await _leaderboardService.Delete(clientId, leaderboardId);
            return Ok();
        }
    }
}