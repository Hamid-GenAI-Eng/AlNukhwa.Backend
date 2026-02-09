using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misan.Modules.Store.Application.Features.Cart.Commands;
using Misan.Modules.Store.Application.Features.Cart.Queries;
using System;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Infrastructure.Controllers;

[Route("api/cart")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ISender _sender;

    public CartController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId()
    {
         var claim = User.FindFirst("userId") ?? User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
         return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        var query = new GetCartQuery(userId);
        var result = await _sender.Send(query);
        return Ok(result.Value);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        var userId = GetUserId();
        var command = new AddToCartCommand(userId, request.ProductId, request.Quantity);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveFromCart(Guid itemId)
    {
        var userId = GetUserId();
        var command = new RemoveFromCartCommand(userId, itemId);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}

public record AddToCartRequest(Guid ProductId, int Quantity);
