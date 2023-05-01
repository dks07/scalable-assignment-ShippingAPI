namespace ShippingAPI.Settings;

public interface IShippingDatabaseSettings
{
  string ShippingCollectionName { get; set; }
  string ConnectionString { get; set; }
  string DatabaseName { get; set; }
}