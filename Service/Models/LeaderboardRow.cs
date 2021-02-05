using System.ComponentModel.DataAnnotations;

namespace LeaderboardAPI.Interfaces
{
    public class LeaderboardRowDTO
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid ClientId supplied")]
        public virtual long ClientId {get; set;}

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid rating supplied")]
        public virtual long Rating {get; set;}
    }
}