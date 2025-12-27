using ECommerce.Domain.Entities;
using ECommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Extensions;

public static class QueryableProductExtensions
{
    public static IQueryable<Product> ApplyFiltering(this IQueryable<Product> q, ProductQuery p)
    {
        if (!string.IsNullOrWhiteSpace(p.Q))
            q = q.Where(x => x.Name.Contains(p.Q)); //select only the products that match my certain condition. Q is the search term, it is defined in my model productQuery and it can be a laptop or a chair etc. So it will filter my query and get only the products that i want.

        if (!string.IsNullOrWhiteSpace(p.Category))
            q = q.Where(x => x.Category == p.Category); //select only the category that i want , like furniture.

        if (p.MinPrice.HasValue)
            q = q.Where(x => x.Price >= p.MinPrice.Value);

        if (p.MaxPrice.HasValue)
            q = q.Where(x => x.Price <= p.MaxPrice.Value);

        return q;
    }

    public static IQueryable<Product> ApplySorting(this IQueryable<Product> q, ProductQuery p)
    {
        var sort = (p.Sort ?? "name").ToLower();
        var dir  = (p.Dir  ?? "asc").ToLower();

        return (sort, dir) switch
        {
            ("price", "desc") => q.OrderByDescending(x => x.Price),
            ("price", _)      => q.OrderBy(x => x.Price),

            ("name", "desc")  => q.OrderByDescending(x => x.Name),
            _                 => q.OrderBy(x => x.Name),
        };
    }
}
