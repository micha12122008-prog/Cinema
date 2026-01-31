using Cinema.Domain.Entities.Enums;
namespace Cinema.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    
    public int SeatNumber { get; set; }
    public decimal Price { get; set; } 
    
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public TicketStatus Status { get; set; } = TicketStatus.Active;
    
    public User User { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}