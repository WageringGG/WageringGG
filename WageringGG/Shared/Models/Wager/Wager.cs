using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WageringGG.Shared.Models
{
    public class Wager : Mode
    {
        public string AccountId { get; set; }
        public StellarAccount Account { get; set; }
        public List<WagerMember> Members { get; set; }
        public List<WagerChallenge> Challenges { get; set; }

        [Column(TypeName = "decimal(18,7)")]
        public decimal Amount { get; set; }
        public int PlayerCount { get; set; }
        public int ChallengeCount { get; set; }

        private WagerMember[] hosts = null;
        public WagerMember[] Hosts()
        {
            if (hosts != null)
                return hosts;
            if (Members == null)
                return null;
            return hosts = Members.Where(x => x.IsHost).ToArray();
        }

        private WagerMember[] clients = null;
        public WagerMember[] Clients()
        {
            if (clients != null)
                return hosts;
            if (Members == null)
                return null;
            return clients = Members.Where(x => !x.IsHost).ToArray();
        }

        public static string Group(int id)
        {
            return $"wager_{id}";
        }

        public override string[] HostIds()
        {
            return Hosts().Select(x => x.ProfileId).ToArray();
        }

        public override string[] ClientIds()
        {
            return Clients().Select(x => x.ProfileId).Distinct().ToArray();
        }

        public override string[] AllIds()
        {
            return Members.Select(x => x.ProfileId).Distinct().ToArray();
        }
    }
}