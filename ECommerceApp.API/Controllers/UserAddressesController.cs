using ECommerceApp.Application.DTOs.Addresses;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAddressesController : ControllerBase
    {
        private readonly IUserAddressService _userAddressService;

        public UserAddressesController(IUserAddressService userAddressService)
        {
            _userAddressService = userAddressService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserAddressDto>>> GetMyAddresses(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var addresses = await _userAddressService.GetMyAddressesAsync(userId, cancellationToken);
            return Ok(addresses);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<UserAddressDto>> GetMyAddressById(long id, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var address = await _userAddressService.GetMyAddressByIdAsync(userId, id, cancellationToken);
            return Ok(address);
        }

        [HttpPost]
        public async Task<ActionResult<UserAddressDto>> Create([FromBody] CreateUserAddressDto request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var createdAddress = await _userAddressService.CreateAsync(userId, request, cancellationToken);

            return CreatedAtAction(
                nameof(GetMyAddressById),
                new { id = createdAddress.Id },
                createdAddress);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<UserAddressDto>> Update(long id, [FromBody] UpdateUserAddressDto request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var updatedAddress = await _userAddressService.UpdateAsync(userId, id, request, cancellationToken);
            return Ok(updatedAddress);
        }

        [HttpPatch("{id:long}/set-default")]
        public async Task<IActionResult> SetDefault(long id, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            await _userAddressService.SetDefaultAsync(userId, id, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            await _userAddressService.DeleteAsync(userId, id, cancellationToken);
            return NoContent();
        }

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException("User id claim not found.");
            }

            return userId;
        }
    }

}
