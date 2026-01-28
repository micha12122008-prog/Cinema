using Cinema.Application.DTO.Movie;
using Cinema.Domain.Entities;

namespace Cinema.Application.Services.Interfaces;

public interface IMovieService
{
    Task<List<Movie>> GetAllAsync();
    Task<Movie> AddAsync(CreateMovieRequest request);
}