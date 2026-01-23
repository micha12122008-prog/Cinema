using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}