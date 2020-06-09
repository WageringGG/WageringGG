using System.Linq;
using WageringGG.Shared.Models;

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
            },
            new Game
            {
                Id = 3,
                Name = "Apex Legends",
                NormalizedName = "apex-legends"
            },
            new Game
            {
                Id = 4,
                Name = "Valorant",
                NormalizedName = "valorant"
            }
        };

        public static int? GetId(string normalizedName)
        {
            return Values.FirstOrDefault(x => x.NormalizedName == normalizedName)?.Id;
        }

        public static string GetName(string normalizedName)
        {
            return Values.FirstOrDefault(x => x.NormalizedName == normalizedName)?.Name;
        }

        public static string GetName(int id)
        {
            return Values.FirstOrDefault(x => x.Id == id)?.Name;
        }
    }
}
