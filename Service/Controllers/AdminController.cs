using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using LeaderboardAPI.Interfaces;


namespace LeaderboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public AdminController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _leaderboardService.DeleteAll();
            return Ok();
        }
    }
}