using System;
using System.Collections.Generic;

namespace WageringGG.Shared.Models
{
    public class PaginatedList<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public List<T> List { get; set; } = new List<T>();

        public PaginatedList() { }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            List = items;
        }
    }
}
