using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

#nullable disable
namespace WageringGG.Shared.Models
{
    public class WagerChallenge : Approvable
    {
        public int Id { get; set; }

        public int WagerId { get; set; }
        public Wager Wager { get; set; }
        public List<WagerMember> Members { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public bool IsAccepted { get; set; }

        public static string Group(int id)
        {
            return $"wager_challenge_{id}";
        }

        public string[] Ids()
        {
            return Members.Select(x => x.ProfileId).ToArray();
        }
    }
}
