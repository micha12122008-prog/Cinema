namespace Cinema.Application.DTO.Ticket;

public class BookTicketRequest
{
    public Guid MovieId { get; set; }
    public int SeatNumber { get; set; }
    public decimal Price { get; set; }
}