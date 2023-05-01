using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ShippingAPI.Models;

public class Shipping
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }

  public string OrderId { get; set; }

  public string UserId { get; set; }

  public DateTime ShippingDate { get; set; }

  public string ShippingAddress { get; set; }

  public string TrackingNumber { get; set; }
}