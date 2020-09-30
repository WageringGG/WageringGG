namespace WageringGG.Shared.Models
{
    public class ChallengeEntry
    {
        public int HostId { get; set; }
        public WagerHost Host { get; set; }
        public int ChallengeId { get; set; }
        public WagerChallenge Challenge { get; set; }

        public byte Entries { get; set; }
    }
}
