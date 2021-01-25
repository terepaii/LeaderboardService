using LeaderboardAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LeaderboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly LeaderboardService _leaderboardService;

        public AdminController(LeaderboardService leaderboardService)
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