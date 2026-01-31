namespace Cinema.Application.DTO.Ticket;

public class TicketRequest
{
    public Guid MovieId { get; set; }
    public int SeatNumber { get; set; }
    public decimal Price { get; set; }
}