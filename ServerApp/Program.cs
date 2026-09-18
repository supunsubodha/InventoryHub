using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Register in-memory cache for frequently requested product data.
builder.Services.AddMemoryCache();

// Allow the Blazor frontend to call this API during local development.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();

// Return a cached list of products. The data is generated once and reused for 5 minutes.
app.MapGet("/api/productlist", (IMemoryCache cache) =>
{
    const string cacheKey = "product-list";

    if (!cache.TryGetValue(cacheKey, out object[]? products))
    {
        products =
        [
            new
            {
                Id = 1,
                Name = "Laptop",
                Price = 1200.50,
                Stock = 25,
                Category = new { Id = 101, Name = "Electronics" }
            },
            new
            {
                Id = 2,
                Name = "Headphones",
                Price = 50.00,
                Stock = 100,
                Category = new { Id = 102, Name = "Accessories" }
            }
        ];

        cache.Set(cacheKey, products, TimeSpan.FromMinutes(5));
    }

    return products;
});

app.Run();