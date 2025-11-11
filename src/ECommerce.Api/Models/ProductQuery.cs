namespace ECommerce.Api.Models;

public sealed class ProductQuery  // the user through swagger hits my get endpoint and requests a filltered return product query
{                                 // the request comes back to this ProductQuery like p = { Q = "chair", Category = "furniture" , MaxPrice = 40}  
    // that p parameter from this ProductQuery will then get passed through q = q.ApplyFiltering(p) inside my controller to QueryableProductExtensions
    // then inside the QueryableProductExtensions the methods get implemented and return a q query that later inside my controller through asynchronous programming becomes a list and gets sent to SQL Server.
    //SQL returns the matching rows and EF Core maps them back to Product objects. Finally with Ok(items) i return them to the user.
    private const int MaxPageSize = 100;

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Sort { get; init; } = "name";
    public string? Dir { get; init; } = "asc";
    public string? Q { get; init; }
    public string? Category { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }

    public int EffectivePageSize =>
        Math.Min(PageSize <= 0 ? 20 : PageSize, MaxPageSize);
}
