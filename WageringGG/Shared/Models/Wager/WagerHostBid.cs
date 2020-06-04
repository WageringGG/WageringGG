#nullable disable

using System.Text.Json.Serialization;

namespace WageringGG.Shared.Models
{
    public class WagerHostBid : WagerBid
    {
        public int WagerId { get; set; }
        [JsonIgnore]
        public Wager Wager { get; set; }
    }
}
