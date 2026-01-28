using Cinema.Domain.Entities;

namespace Cinema.Application.Services.Interfaces;

public interface ITicketService
{
    Task<Ticket> BookTicketAsync(Guid userId, Guid movieId);
    Task<List<Ticket>> GetUserTicketsAsync(Guid userId);
    Task<List<Ticket>> GetAllTicketsAsync();
    Task CancelTicketAsync(Guid ticketId, Guid userId, string userRole);
}