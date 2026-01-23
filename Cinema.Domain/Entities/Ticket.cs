using Cinema.Domain.Entities.Enums;
using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public TicketStatus Status { get; set; }
    public User User { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}