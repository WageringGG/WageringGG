﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WageringGG.Shared.Models
{
    public class Wager : Mode
    {
        public List<WagerHostBid> Hosts { get; set; } = new List<WagerHostBid>();
        public List<WagerChallenge> Challenges { get; set; } = new List<WagerChallenge>();

        [Column(TypeName = "decimal(18,7)")]
        public decimal? MinimumWager { get; set; }
        [Column(TypeName = "decimal(18,7)")]
        public decimal? MaximumWager { get; set; }
        public int PlayerCount { get; set; }
        public int ChallengeCount { get; set; }

        public override bool IsApproved()
        {
            if (Status == 1)
                return true;

            if (Hosts == null || Hosts.Count == 0)
                return false;
            foreach (WagerHostBid bid in Hosts)
                if (bid.Approved == null || bid.Approved == false)
                    return false;
            return true;
        }

        public override string GroupName
        {
            get
            {
                return GetGroupName.Wager(Id);
            }
        }

        public override IEnumerable<string> HostIds()
        {
            return Hosts.Select(x => x.ProfileId);
        }

        public override IEnumerable<string> ClientIds()
        {
            IEnumerable<string> result = Enumerable.Empty<string>();
            foreach (WagerChallenge challenge in Challenges)
            {
                result = result.Union(challenge.ChallengerIds());
            }
            return result.Distinct();
        }

        public override IEnumerable<string> AllIds()
        {
            return HostIds().Union(ClientIds());
        }
    }
}