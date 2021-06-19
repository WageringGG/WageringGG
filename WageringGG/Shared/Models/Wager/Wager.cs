using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WageringGG.Shared.Models
{
    public class Wager : Mode
    {
        public Wager()
        {
            Hosts = new HashSet<WagerHost>();
            Challenges = new List<WagerChallenge>();
        }

        public ICollection<WagerHost> Hosts { get; set; }
        public ICollection<WagerChallenge> Challenges { get; set; }

        [Column(TypeName = "decimal(18,7)")]
        public decimal Amount { get; set; }
        public int PlayerCount { get; set; }
        public int ChallengeCount { get; set; }
    }
}