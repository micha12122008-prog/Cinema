using Cinema.Domain.Entities;

namespace Cinema.Application.Services.Interfaces;

public interface ITicketService
{
    Task<Ticket> BookTicketAsync(Guid userId, Guid movieId, int seatNumber, decimal price);
    Task<IEnumerable<Ticket>> GetAllTicketsAsync();
    Task<IEnumerable<Ticket>> GetUserTicketsAsync(Guid userId);
    Task CancelTicketAsync(Guid ticketId, Guid userId, string role);
}