using System;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Handlers
{
    public static class RatingHandler
    {
        public static decimal GetNewRating(this Rating r1, Rating r2, bool isWin)
        {
            //take wager amount into consideration
            return r1.Value + Rating.K * ((isWin ? 1 : 0) - GetExpectedOutcome(r1, r2));
        }

        public static decimal GetExpectedOutcome(Rating r1, Rating r2)
        {
            decimal x = r1.Value - r2.Value;
            return (decimal)(1 / (1 + Math.Pow(10, -1 * (double)(x / Rating.N))));
        }
    }
}
