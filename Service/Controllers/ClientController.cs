using LeaderboardAPI.Models;
using LeaderboardAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaderboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly LeaderboardService _leaderboardService;

        public ClientController(LeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Row>>> GetAllRows() 
        {
            var result = await _leaderboardService.Get(null);
            if (result == null)
            {
                return NoContent();
            }
            return result;
        }
            

        [HttpGet("{clientId}")]
        public async Task<ActionResult<List<Row>>> GetRow([FromRoute] long clientId)
        {
            var result = await _leaderboardService.Get(clientId);

            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Row row)
        {
            var result = await _leaderboardService.Get(row.ClientId);

            if (result.Count == 0)
            {
                await _leaderboardService.Create(row);
                return Ok();
            }

            return Conflict(new { message = $"An existing enttry with from client {row.ClientId} was found" });
        }

        [HttpPut]
        public async Task<IActionResult> Update(Row rowIn)
        {
            var result = await _leaderboardService.Get(rowIn.ClientId);

            if (result == null)
            {
                return NotFound();
            }

            await _leaderboardService.Update(rowIn);
            return Ok();
        }

        [HttpDelete("{clientId}")]
        public async Task<IActionResult> Delete([FromRoute] long clientId)
        {
            var row = await _leaderboardService.Get(clientId);

            if (row == null)
            {
                return NotFound();
            }

            await _leaderboardService.Delete(clientId);
            return Ok();
        }
    }
}