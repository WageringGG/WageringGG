using WageringGG.Shared.Models;
using System.Linq;

namespace WageringGG.Shared.Constants
{
    public class Games
    {
        public static Game[] Values = new Game[]
        {
            new Game
            {
                Id = 1,
                Name = "Fortnite",
                NormalizedName = "fortnite"
            },
            new Game
            {
                Id = 2,
                Name = "Modern Warfare",
                NormalizedName = "modern-warfare"
            }
        };

        public static int? GetId(string normalizedName)
        {
            return Values.FirstOrDefault(x => x.NormalizedName == normalizedName)?.Id;
        }
    }
}
