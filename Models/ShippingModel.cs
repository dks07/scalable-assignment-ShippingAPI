namespace ShippingAPI.Models;

public class ShippingModel
{
  public string Id { get; set; }

  public string OrderId { get; set; }

  public string UserId { get; set; }

  public DateTime ShippingDate { get; set; }

  public string ShippingAddress { get; set; }

  public string TrackingNumber { get; set; }
}