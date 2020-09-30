using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WageringGG.Shared.Models
{
    public class Wager : Mode
    {
        public List<WagerHost> Hosts { get; set; }
        public List<WagerChallenge> Challenges { get; set; }

        [Column(TypeName = "decimal(18,7)")]
        public decimal Amount { get; set; }
        public int PlayerCount { get; set; }
        public int ChallengeCount { get; set; }

        public static string Group(int id)
        {
            return $"wager_{id}";
        }

        private string[] hostIds;
        public string[] HostIds()
        {
            if (hostIds == null)
                hostIds = Hosts.Select(x => x.ProfileId).ToArray();
            return hostIds;
        }
    }
}