namespace WageringGG.Shared.Models
{
    public class WagerApproval
    {
        public int HostId { get; set; }
        public WagerHostBid Host { get; set; }
        public int ChallengeId { get; set; }
        public WagerChallenge Challenge { get; set; }
        public bool? Approved { get; set; }
    }
}
