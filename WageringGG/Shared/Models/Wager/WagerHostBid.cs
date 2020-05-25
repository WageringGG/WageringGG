#nullable disable

using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class WagerHostBid : WagerBid
    {
        public int WagerId { get; set; }
        public Wager Wager { get; set; }

        [NotMapped]
        public string DisplayName { get; set; }
    }
}
