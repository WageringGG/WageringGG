using System.Text.Json.Serialization;

namespace WageringGG.Shared.Models
{
    public class WagerApproval
    {
        public int HostId { get; set; }
        [JsonIgnore]
        public WagerHostBid Host { get; set; }
        public int ChallengeId { get; set; }
        [JsonIgnore]
        public WagerChallenge Challenge { get; set; }
        public bool? Approved { get; set; }
    }
}
