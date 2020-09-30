using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WageringGG.Shared.Models
{
    public class WagerChallenge : Approvable
    {
        public int Id { get; set; }

        public int WagerId { get; set; }
        public Wager Wager { get; set; }
        public string AccountId { get; set; }
        public StellarAccount Account { get; set; }
        public List<WagerChallenger> Challengers { get; set; }
        public List<ChallengeEntry> Entries { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public bool? IsAccepted { get; set; }

        public static string Group(int id)
        {
            return $"wager_challenge_{id}";
        }

        private string[] challengerIds;
        public string[] ChallengerIds()
        {
            if (challengerIds == null)
                challengerIds = Challengers.Select(x => x.ProfileId).ToArray();
            return challengerIds;
        }
    }
}
