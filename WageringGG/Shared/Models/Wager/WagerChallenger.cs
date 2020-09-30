namespace WageringGG.Shared.Models
{
    public class WagerChallenger : WagerMember
    {
        public int ChallengeId { get; set; }
        public WagerChallenge Challenge { get; set; }

        public int Entries { get; set; }
    }
}
