using Cinema.Application.DTO.Ticket;
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

    public TicketsController(ITicketService ticketService) => _ticketService = ticketService;

    [HttpPost]
    public async Task<IActionResult> BookTicket([FromBody] BookTicketRequest request)
    {
        var userId = GetUserId();
        var ticket = await _ticketService.BookTicketAsync(userId, request.MovieId, request.SeatNumber, request.Price);
        return Ok(ticket);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetMyTickets()
    {
        var userId = GetUserId();
        var tickets = await _ticketService.GetUserTicketsAsync(userId);
        return Ok(tickets);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllTickets() => Ok(await _ticketService.GetAllTicketsAsync());

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelTicket(Guid id)
    {
        var userId = GetUserId();
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "Customer";
        await _ticketService.CancelTicketAsync(id, userId, role);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? User.FindFirstValue("id");
        return Guid.TryParse(id, out var guid) ? guid : throw new UnauthorizedAccessException("User ID not found in token");
    }
}