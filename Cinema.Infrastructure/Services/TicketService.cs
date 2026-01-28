using Cinema.Application.Services.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Domain.Entities.Enums;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Cinema.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    
    private string GetUserCacheKey(Guid userId) => $"tickets_user_{userId}";

    public TicketService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Ticket> BookTicketAsync(Guid userId, Guid movieId)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MovieId = movieId,
            PurchaseDate = DateTime.UtcNow,
            Status = TicketStatus.Active
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        _cache.Remove(GetUserCacheKey(userId));

        return ticket;
    }

    public async Task<List<Ticket>> GetUserTicketsAsync(Guid userId)
    {
        string cacheKey = GetUserCacheKey(userId);

        if (!_cache.TryGetValue(cacheKey, out List<Ticket> tickets))
        {
            tickets = await _context.Tickets
                .Where(t => t.UserId == userId)
                .Include(t => t.Movie)
                .ToListAsync();

            _cache.Set(cacheKey, tickets, TimeSpan.FromMinutes(2));
        }

        return tickets!;
    }

    public async Task<List<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets
            .Include(t => t.Movie)
            .Include(t => t.User)
            .ToListAsync();
    }

    public async Task CancelTicketAsync(Guid ticketId, Guid userId, string userRole)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        
        if (ticket == null) throw new Exception("Квиток не знайдено");
        
        if (userRole != "Admin" && ticket.UserId != userId)
            throw new Exception("У вас немає прав для скасування цього квитка");

        ticket.Status = TicketStatus.Cancelled;
        await _context.SaveChangesAsync();

        _cache.Remove(GetUserCacheKey(ticket.UserId));
    }
}