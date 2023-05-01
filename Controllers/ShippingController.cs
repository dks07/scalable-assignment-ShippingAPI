using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingAPI.Models;
using ShippingAPI.Services;

namespace ShippingAPI.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class ShippingController : ControllerBase
  {
    private readonly IShippingService _shippingService;

    public ShippingController(IShippingService shippingService)
    {
      _shippingService = shippingService;
    }

    // GET: api/Shipping
    [HttpGet]
    public async Task<ActionResult<List<ShippingModel>>> Get()
    {
      var shippings = await _shippingService.GetAllShippingsAsync();
      return Ok(shippings);


    }

    // GET: api/Shipping/5
    [HttpGet("{id}", Name = "GetShippingById")]
    [ActionName("GetShippingById")]
    public async Task<ActionResult<ShippingModel>> Get(string id)
    {
      var shipping = await _shippingService.GetShippingByIdAsync(id);
      if (shipping == null)
      {
        return NotFound();
      }

      return Ok(shipping);
    }

    // POST: api/Shipping
    [HttpPost]
    public async Task<ActionResult<ShippingModel>> Post([FromBody] ShippingModel shippingModel)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      
      var shipping = await _shippingService.CreateShippingAsync(shippingModel);
      return CreatedAtRoute("GetShippingById", new { id = shipping.Id }, shipping);
    }

    // PUT: api/Shipping/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] ShippingModel shippingModel)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var shipping = await _shippingService.GetShippingByIdAsync(id);
      if (shipping == null)
      {
        return NotFound();
      }

      await _shippingService.UpdateShippingAsync(id, shippingModel);

      return Ok();
    }

    // DELETE: api/Shipping/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      var result = await _shippingService.DeleteShippingAsync(id);
      if (!result)
      {
        return NotFound();
      }
      return Ok();
    }
  }

}
