using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misan.Modules.Store.Application.Features.Wishlist.Commands;
using Misan.Modules.Store.Application.Features.Wishlist.Queries;
using System;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Infrastructure.Controllers;

[Route("api/wishlist")]
[ApiController]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly ISender _sender;

    public WishlistController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId()
    {
         var claim = User.FindFirst("userId") ?? User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
         return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var query = new GetWishlistQuery(GetUserId());
        var result = await _sender.Send(query);
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistRequest request)
    {
        var command = new AddToWishlistCommand(GetUserId(), request.ItemType, request.ItemId);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("{itemId}")]
    public async Task<IActionResult> RemoveFromWishlist(Guid itemId)
    {
        var command = new RemoveFromWishlistCommand(GetUserId(), itemId);
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    [HttpGet("check/{type}/{id}")]
    public async Task<IActionResult> CheckWishlistStatus(string type, Guid id)
    {
        var query = new CheckWishlistStatusQuery(GetUserId(), type, id);
        var result = await _sender.Send(query);
        return Ok(new { Exists = result.Value });
    }
}

public record AddWishlistRequest(string ItemType, Guid ItemId);
