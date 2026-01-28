using Cinema.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cinema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    public async Task<IActionResult> BookTicket([FromBody] Guid movieId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ticket = await _ticketService.BookTicketAsync(userId, movieId);
        return Ok(ticket);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")] // Тільки для адмінів
    public async Task<IActionResult> GetAllTickets()
    {
        return Ok(await _ticketService.GetAllTicketsAsync());
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetMyTickets()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _ticketService.GetUserTicketsAsync(userId));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelTicket(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userRole = User.FindFirstValue(ClaimTypes.Role)!;
        
        await _ticketService.CancelTicketAsync(id, userId, userRole);
        return NoContent();
    }
}