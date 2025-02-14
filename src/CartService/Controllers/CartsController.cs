using AutoMapper;
using CartService.DTOs;
using CartService.Entities;
using CartService.Interfaces;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers;

[Authorize]
public class CartsController(ICartRepository cartRepository, IBookRepository bookRepository,
    IMapper mapper, IPublishEndpoint publishEndpoint) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartDto>>> GetCarts()
    {
        var identity = User.Identity;
        if (identity == null || identity.Name == null)
            return BadRequest("Failed to get identity of a user");

        var carts = await cartRepository.GetCartsAsync(identity.Name);
        
        return Ok(carts);
    }

    [HttpGet("{cartId}")]
    public async Task<ActionResult<CartDto>> GetCart(Guid cartId)
    {
        var identity = User.Identity;
        if (identity == null || identity.Name == null)
            return BadRequest("Failed to get identity of a user");

        var cart = await cartRepository.GetCartWithDetailsByIdAsync(cartId);
        if (cart == null) return NotFound();
        if (cart.Username != identity.Name) return Unauthorized();

        return Ok(mapper.Map<CartDto>(cart));
    }

    [HttpPut("add")]
    public async Task<ActionResult<CartDto>> AddToCart(Guid bookId, int quantity)
    {
        var identity = User.Identity;
        if (identity == null || identity.Name == null)
            return BadRequest("Failed to get identity of a user");

        if (quantity < 1) return BadRequest("Quantity has to be greater or equal to 1");

        var book = await bookRepository.GetBookByIdAsync(bookId);
        if (book == null) return BadRequest("Failed to find book of given id");

        var cart = await cartRepository.GetActiveOrProceedingCartByUsernameAsync(identity.Name);
        if (cart == null) // No cart yet, its first book user adds to cart
        {
            var newCart = new Cart
            {
                Username = identity.Name
            };

            cartRepository.AddCart(newCart);

            var item = new BookCart
            {
                Quantity = quantity,
                Book = book,
                CartId = newCart.Id
            };

            newCart.BookCarts.Add(item);

            var cartToPublish = mapper.Map<CartDto>(newCart);
            await publishEndpoint.Publish(mapper.Map<CartCreated>(cartToPublish));
        }
        else if (cart.Status == CartStatus.Proceeding) // Cart exists but is already proceeding
        {
            return BadRequest("This cart is already proceeding");
        }
        else // Cart exists
        {
            var item = await cartRepository.GetBookCartByIdsAsync(cart.Id, bookId);
            if (item == null) // New book in cart, add it to cart
            {
                var newItem = new BookCart
                {
                    Quantity = quantity,
                    Book = book,
                    CartId = cart.Id
                };

                cart.BookCarts.Add(newItem);
                cart.UpdatedAt = DateTime.UtcNow;
            }
            else // Book exists in cart, increase quantity
            {
                item.Quantity += quantity;
                cart.UpdatedAt = DateTime.UtcNow;
            }

            var cartToPublish = mapper.Map<CartDto>(cart);
            await publishEndpoint.Publish(mapper.Map<CartUpdated>(cartToPublish));
        }

        if (await cartRepository.Complete())
            return NoContent();
        return BadRequest("Failed to add items to cart");
    }

    [HttpPut("remove")]
    public async Task<ActionResult<CartDto>> RemoveFromCart(Guid bookId, int quantity)
    {
        var identity = User.Identity;
        if (identity == null || identity.Name == null)
            return BadRequest("Failed to get identity of a user");

        if (quantity < 1) return BadRequest("Quantity has to be greater or equal to 1");

        var book = await bookRepository.GetBookByIdAsync(bookId);
        if (book == null) return BadRequest("Failed to find book of given id");

        var cart = await cartRepository.GetActiveOrProceedingCartByUsernameAsync(identity.Name);
        if (cart == null)
        {
            return BadRequest("Cart does not exist");
        }
        if (cart.Status == CartStatus.Proceeding) // Cart exists but is already proceeding
        {
            return BadRequest("This cart is already proceeding");
        }
        
        var item = await cartRepository.GetBookCartByIdsAsync(cart.Id, bookId);
        if (item == null)
        {
           return BadRequest("Book does not exist in cart");
        }

        item.Quantity -= quantity;

        if (item.Quantity < 1)
        {
            cart.BookCarts.Remove(item);
        }
        cart.UpdatedAt = DateTime.UtcNow;

        var cartToPublish = mapper.Map<CartDto>(cart);
        await publishEndpoint.Publish(mapper.Map<CartUpdated>(cartToPublish));
        
        if (await cartRepository.Complete())
            return NoContent();
        return BadRequest("Failed to remove items from cart");
    }
}
