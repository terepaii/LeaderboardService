using System;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations;

namespace LeaderboardAPI.Interfaces
{
    public class LeaderboardRowDTO
    {
        public Guid ClientId {get; set;}

        [Required]
        // TODO: Add an id validator
        public short LeaderboardId {get; set;}

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid rating supplied")]
        public long Rating {get; set;}        
    }
}