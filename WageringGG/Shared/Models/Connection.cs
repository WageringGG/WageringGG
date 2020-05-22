using System.ComponentModel.DataAnnotations;
#nullable disable

namespace WageringGG.Shared.Models
{
    public class Connection
    {
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        [Required]
        public string ConnectionId { get; set; }
        [Required]
        public string UserAgent { get; set; }
        public bool Connected { get; set; }
    }
}