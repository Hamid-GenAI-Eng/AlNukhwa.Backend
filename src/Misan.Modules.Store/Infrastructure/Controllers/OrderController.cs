using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misan.Modules.Store.Application.Features.Orders.Commands;
using Misan.Modules.Store.Application.Features.Orders.Queries;
using System;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Infrastructure.Controllers;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly ISender _sender;

    public OrderController(ISender sender)
    {
        _sender = sender;
    }
    
    private Guid GetUserId()
    {
         var claim = User.FindFirst("userId") ?? User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
         return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userId = GetUserId();
        // Assuming Address ID is passed or default used.
        // For simplicity, requesting AddressId.
        var command = new CreateOrderCommand(userId, request.ShippingAddressId, "Payfast");
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(new { OrderId = result.Value }) : BadRequest(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderHistory()
    {
        // TODO: Implement GetOrdersQuery
        // For now returning empty list or implemented query? 
        // User asked for GET /api/orders
        return Ok(new object[] { });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var query = new GetOrderQuery(id, GetUserId());
        var result = await _sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        var command = new CancelOrderCommand(id, GetUserId());
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpGet("{id}/invoice")]
    public async Task<IActionResult> GetInvoice(Guid id)
    {
        var query = new GetOrderInvoiceQuery(id, GetUserId());
        var result = await _sender.Send(query);
        return result.IsSuccess ? Ok(new { Url = result.Value }) : BadRequest(result.Error);
    }

    [HttpGet("{id}/track")]
    public async Task<IActionResult> TrackShipment(Guid id)
    {
        var query = new GetOrderTrackingQuery(id, GetUserId());
        var result = await _sender.Send(query);
        return result.IsSuccess ? Ok(new { TrackingNumber = result.Value }) : BadRequest(result.Error);
    }
}

public record CreateOrderRequest(Guid ShippingAddressId);
