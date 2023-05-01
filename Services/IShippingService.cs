using ShippingAPI.Models;

namespace ShippingAPI.Services
{
  public interface IShippingService
  {
    Task<List<ShippingModel>> GetAllShippingsAsync();

    Task<ShippingModel?> GetShippingByIdAsync(string id);

    Task<ShippingModel> CreateShippingAsync(ShippingModel shippingDto);

    Task UpdateShippingAsync(string id, ShippingModel shippingDto);

    Task<bool> DeleteShippingAsync(string id);
  }
}