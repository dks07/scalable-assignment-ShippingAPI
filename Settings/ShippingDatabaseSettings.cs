namespace ShippingAPI.Settings
{
  public class ShippingDatabaseSettings : IShippingDatabaseSettings
  {
    public string ShippingCollectionName { get; set; }
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
  }
}
