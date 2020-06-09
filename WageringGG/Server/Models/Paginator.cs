using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WageringGG.Shared.Models;

namespace WageringGG.Server.Models
{
    public static class Paginator<T>
    {
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
