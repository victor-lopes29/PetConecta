using FrontPetConecta.Data;
using FrontPetConecta.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FrontPetConecta.Pages.Vendas
{
    public class VendasModel : PageModel
    {
        public List<VendaModel> VendaList { get; set; } = new List<VendaModel>();

        public VendasModel() { }

        public async Task<IActionResult> OnGetAsync()
        {
            using var httpClient = new HttpClient();
            var url = "http://localhost:5151/api/Vendas";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            try
            {
                var response = await httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (content.StartsWith("{") || content.StartsWith("["))
                    {
                        var receber_content = JsonConvert.DeserializeObject(content);
                        VendaList = JsonConvert.DeserializeObject<List<VendaModel>>(content);
                    }
                    else
                    {
                        Console.WriteLine($"Non-JSON content received: {content}");
                    }
                }
                else
                {
                    Console.WriteLine($"HTTP error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return Page();
        }
    }
}