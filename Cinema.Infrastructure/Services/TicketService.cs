using Cinema.Application.Services.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Domain.Entities.Enums;
using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;

    public TicketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket> BookTicketAsync(Guid userId, Guid movieId, int seatNumber, decimal price)
    {
        var movieExists = await _context.Movies.AnyAsync(m => m.Id == movieId);
        if (!movieExists)
        {
            throw new Exception("Фільм з таким ID не знайдено.");
        }

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MovieId = movieId,
            SeatNumber = seatNumber,
            Price = price,
            PurchaseDate = DateTime.UtcNow,
            Status = TicketStatus.Active
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return ticket;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets
            .Include(t => t.Movie)
            .Include(t => t.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(Guid userId)
    {
        return await _context.Tickets
            .Where(t => t.UserId == userId)
            .Include(t => t.Movie)
            .ToListAsync();
    }

    public async Task CancelTicketAsync(Guid ticketId, Guid userId, string role)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        
        if (ticket == null)
        {
            throw new Exception("Квиток не знайдено.");
        }

        if (role == "Admin" || ticket.UserId == userId)
        {
            ticket.Status = TicketStatus.Cancelled;
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new UnauthorizedAccessException("У вас немає прав для скасування цього квитка.");
        }
    }
}