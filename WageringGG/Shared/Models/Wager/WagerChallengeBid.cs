#nullable disable
namespace WageringGG.Shared.Models
{
    public class WagerChallengeBid : WagerBid
    {
        public int ChallengeId { get; set; }
        public WagerChallenge Challenge { get; set; }
    }
}
