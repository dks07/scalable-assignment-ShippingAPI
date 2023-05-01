using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShippingAPI.Models;
using ShippingAPI.Settings;
using System.Text;

namespace ShippingAPI.Services
{
  public class ShippingService : IShippingService
  {
    private readonly IMongoCollection<Shipping> _shippings;
    private readonly IConnection _rabbitMQConnection;
    private readonly IModel _rabbitMQChannel;

    public ShippingService(IMongoClient client, IShippingDatabaseSettings settings, IConnection rabbitMQConnection,
      IModel rabbitMQChannel)
    {
      var database = client.GetDatabase(settings.DatabaseName);

      _shippings = database.GetCollection<Shipping>(settings.ShippingCollectionName);
      _rabbitMQConnection = rabbitMQConnection;
      _rabbitMQChannel = rabbitMQChannel;
      _rabbitMQChannel.QueueDeclare("shipping", true, false);

      var consumer = new EventingBasicConsumer(_rabbitMQChannel);
      consumer.Received += async (_, ea) =>
      {
        try
        {

          var message = Encoding.UTF8.GetString(ea.Body.ToArray());
          var shippingMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(message);
          if (shippingMessage == null) return;
          if (shippingMessage.Operation == "Create")
          {
            // Create the shipping using the message information
            var shippingModel = new ShippingModel
            {
              UserId = shippingMessage.UserId,
              OrderId = shippingMessage.OrderId,
              ShippingAddress = shippingMessage.ShippingAddress,
            };
            await CreateShippingAsync(shippingModel);
          }
          else if (shippingMessage.Operation == "Delete")
          {
            // Delete the shipping using the message information
            await DeleteShippingByOrderAsync(shippingMessage.OrderId.ToString());
          }
          _rabbitMQChannel.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception e)
        {
          _rabbitMQChannel.BasicNack(ea.DeliveryTag, false, false);
        }
      };
      _rabbitMQChannel.BasicConsume(
        queue: "shipping",
        autoAck: false,
        consumer: consumer);
    }

    public async Task<List<ShippingModel>> GetAllShippingsAsync()
    {
      var cursor = await _shippings.FindAsync(shipping => true);
      var shippings = await cursor.ToListAsync();
      return shippings.Select(shipping =>
        {
          var shippingModel = new ShippingModel()
          {
            UserId = shipping.UserId,
            OrderId = shipping.OrderId,
            ShippingAddress = shipping.ShippingAddress,
            ShippingDate = shipping.ShippingDate,
            TrackingNumber = shipping.TrackingNumber,
            Id = shipping.Id
          };
          return shippingModel;
        })
        .ToList();
    }
    public async Task<ShippingModel?> GetShippingByIdAsync(string id)
    {
      var cursor = await _shippings.FindAsync(s => s.Id == id);
      var shipping = await cursor.FirstOrDefaultAsync();
      if (shipping == null)
      {
        return null;
      }
      else
      {
        var shippingModel = new ShippingModel()
        {
          UserId = shipping.UserId,
          OrderId = shipping.OrderId,
          ShippingAddress = shipping.ShippingAddress,
          ShippingDate = shipping.ShippingDate,
          TrackingNumber = shipping.TrackingNumber,
          Id = shipping.Id
        };
        return shippingModel;
      }
    }

    public async Task<ShippingModel> CreateShippingAsync(ShippingModel shippingModel)
    {
      if (string.IsNullOrEmpty(shippingModel.Id))
      {
        shippingModel.Id = ObjectId.GenerateNewId().ToString();
      }
      await _shippings.InsertOneAsync(new Shipping
      {
        UserId = shippingModel.UserId,
        OrderId = shippingModel.OrderId,
        ShippingAddress = shippingModel.ShippingAddress,
        ShippingDate = DateTime.Now,
        TrackingNumber = new Guid().ToString(),
        Id = shippingModel.Id
      });
      return shippingModel;
    }

    public async Task UpdateShippingAsync(string id, ShippingModel shippingModel)
    {
      await _shippings.ReplaceOneAsync(p => p.Id == id, new Shipping
      {
        UserId = shippingModel.UserId,
        OrderId = shippingModel.OrderId,
        ShippingAddress = shippingModel.ShippingAddress,
        ShippingDate = shippingModel.ShippingDate,
        TrackingNumber = shippingModel.TrackingNumber,
        Id = shippingModel.Id
      });
    }

    public async Task<bool> DeleteShippingAsync(string id)
    {
      var deleteResult = await _shippings.DeleteOneAsync(p => p.Id == id);
      return deleteResult.DeletedCount == 1;
    }

    private async Task<bool> DeleteShippingByOrderAsync(string orderId)
    {
      var deleteResult = await _shippings.DeleteOneAsync(p => p.OrderId == orderId);
      return deleteResult.DeletedCount == 1;
    }
  }
}
