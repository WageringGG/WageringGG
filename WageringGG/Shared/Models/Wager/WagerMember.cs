namespace WageringGG.Shared.Models
{
    public class WagerMember
    {
        public int Id { get; set; }

        public int WagerId { get; set; }
        public Wager Wager { get; set; }
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public int? ChallengeId { get; set; }
        public WagerChallenge Challenge { get; set; }

        public bool IsHost { get; set; }
        public bool? IsApproved { get; set; }

        public byte ReceivablePercentage { get; set; }
        public byte PayablePercentage { get; set; }
    }
}
