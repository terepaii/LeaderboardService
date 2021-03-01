using System;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;

namespace LeaderboardAPI.Interfaces
{
    public class LeaderboardRowDTO
    {
        public virtual Guid ClientId {get; set;}

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid rating supplied")]
        public virtual long Rating {get; set;}
    }
}