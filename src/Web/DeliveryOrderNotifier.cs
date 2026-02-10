using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System.Linq;
 
public class DeliveryOrderNotifier
{
    private readonly HttpClient _httpClient;
    private readonly string _functionUrl;
 
    public DeliveryOrderNotifier(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _functionUrl = configuration["DeliveryFunctionUrl"];
    }
 
    public async Task NotifyAsync(Order order)
    {
        var deliveryOrder = new
        {
            ShippingAddress = new {
                Street = order.ShipToAddress.Street,
                City = order.ShipToAddress.City,
                State = order.ShipToAddress.State,
                Country = order.ShipToAddress.Country,
                ZipCode = order.ShipToAddress.ZipCode
            },
            Items = order.OrderItems.Select(i => new { Name = i.ItemOrdered.ProductName, Quantity = i.Units }),
            FinalPrice = order.Total()
        };
 
        var json = JsonConvert.SerializeObject(deliveryOrder);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_functionUrl, content);
        response.EnsureSuccessStatusCode();
    }
}