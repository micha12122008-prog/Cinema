using Cinema.Application.DTO.Movie;
using Cinema.Application.Services.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Cinema.Infrastructure.Services;

public class MovieService : IMovieService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const string MoviesCacheKey = "movies_all";

    public MovieService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }
    
    public async Task<List<Movie>> GetAllAsync()
    {
        if (!_cache.TryGetValue(MoviesCacheKey, out List<Movie>? movies))
        {
            movies = await _context.Movies
                .Include(m => m.Tickets)
                .AsNoTracking()
                .ToListAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            _cache.Set(MoviesCacheKey, movies, cacheOptions);
        }

        return movies ?? new List<Movie>();
    }
    
    public async Task<Movie> AddAsync(CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Genre = request.Genre,
            DurationMinutes = request.DurationMinutes,
            Description = request.Description
        };

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        
        _cache.Remove(MoviesCacheKey);
        return movie;
    }
}