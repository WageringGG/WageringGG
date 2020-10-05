using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class WagerMember
    {
        public int Id { get; set; }
        public int WagerId { get; set; }
        public Wager Wager { get; set; }
        public int ChallengeId { get; set; }
        public WagerChallenge Challenge { get; set; }
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }

        public bool? IsApproved { get; set; }
        public bool IsHost { get; set; }

        [Column(TypeName = "decimal(18,7)")]
        public decimal Payable { get; set; }
        [Column(TypeName = "decimal(18,7)")]
        public decimal Receivable { get; set; }
        public int Entries { get; set; }
    }
}
