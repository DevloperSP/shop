using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System.Linq;
using Microsoft.Extensions.Configuration; // <-- добавьте этот using
 
public class OrderItemsReserverClient(HttpClient httpClient, IConfiguration configuration)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _functionUrl = configuration["OrderItemsReserverFunctionUrl"];
 
    public async Task ReserveOrderItemsAsync(Order order)
    {
        var orderRequest = new
        {
            OrderId = order.Id,
            Items = order.OrderItems.Select(i => new { ItemId = i.ItemOrdered.CatalogItemId, Quantity = i.Units })
        };
 
        var json = JsonConvert.SerializeObject(orderRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_functionUrl, content);
        response.EnsureSuccessStatusCode();
    }
}