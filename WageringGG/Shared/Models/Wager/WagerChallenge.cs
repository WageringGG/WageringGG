﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

#nullable disable
namespace WageringGG.Shared.Models
{
    public class WagerChallenge : Approvable
    {
        public int Id { get; set; }

        public int WagerId { get; set; }
        [JsonIgnore]
        public Wager Wager { get; set; }

        public List<WagerApproval> Approvals { get; set; } = new List<WagerApproval>();
        public List<WagerChallengeBid> Challengers { get; set; } = new List<WagerChallengeBid>();

        [Required]
        public DateTime Date { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,7)")]
        public decimal Amount { get; set; }
        public bool IsAccepted { get; set; }

        public override string GroupName
        {
            get
            {
                return GetGroupName.WagerChallenge(Id);
            }
        }

        public override bool IsApproved()
        {
            if (Status == 1)
                return true;

            foreach (WagerChallengeBid bid in Challengers)
                if (bid.Approved == null || bid.Approved == false)
                    return false;
            if (Challengers.Count == 0)
                return false;
            return true;
        }

        public IEnumerable<string> ChallengerIds()
        {
            return Challengers.Select(x => x.ProfileId);
        }
    }
}
