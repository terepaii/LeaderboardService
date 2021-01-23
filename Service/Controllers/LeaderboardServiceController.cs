using LeaderboardApi.Models;
using LeaderboardApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LeaderboardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly LeaderboardService _leaderboardService;

        public LeaderboardController(LeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet]
        public ActionResult<List<Row>> GetAllRows() =>
            _leaderboardService.Get();

        [HttpGet("{clientId}")]
        public ActionResult<Row> GetRow([FromRoute] long clientId)
        {
            Row row = _leaderboardService.Get(clientId);

            if (row == null)
            {
                return NotFound();
            }
            return row;
        }

        [HttpPost]
        public IActionResult Post(Row row)
        {
            Row rowEntry = _leaderboardService.Get(row.ClientId);

            if (rowEntry == null)
            {
                _leaderboardService.Create(row);
                return Ok();
            }

            return Conflict(new { message = $"An existing enttry with from client {row.ClientId} was found" });
        }

        [HttpPut]
        public IActionResult Update(Row rowIn)
        {
            Row row = _leaderboardService.Get(rowIn.ClientId);

            if (row == null)
            {
                return NotFound();
            }

            _leaderboardService.Update(rowIn);

            return NoContent();
        }

        [HttpDelete("{clientId}")]
        public IActionResult Delete([FromRoute] long clientId)
        {
            Row row = _leaderboardService.Get(clientId);

            if (row == null)
            {
                return NotFound();
            }

            _leaderboardService.Delete(clientId);

            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteAll()
        {
            _leaderboardService.DeleteAll();
            return NoContent();
        }
    }
}