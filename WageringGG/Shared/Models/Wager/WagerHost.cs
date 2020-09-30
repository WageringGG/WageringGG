using System.Collections.Generic;

namespace WageringGG.Shared.Models
{
    public class WagerHost : WagerMember
    {
        public int WagerId { get; set; }
        public Wager Wager { get; set; }

        public List<ChallengeEntry> Entries { get; set; }
    }
}
