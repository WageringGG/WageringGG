using System.Text.Json.Serialization;

#nullable disable
namespace WageringGG.Shared.Models
{
    public class WagerChallengeBid : WagerBid
    {
        public int ChallengeId { get; set; }
        [JsonIgnore]
        public WagerChallenge Challenge { get; set; }
    }
}
