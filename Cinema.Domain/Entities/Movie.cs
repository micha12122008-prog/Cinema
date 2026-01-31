namespace Cinema.Domain.Entities;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public int DurationMinutes { get; set; }
    public string Description { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore] 
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}