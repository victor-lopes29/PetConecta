using FrontPetConecta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FrontPetConecta.Pages.Vendas
{
    public class DeleteModel : PageModel
    {
        [BindProperty]
        public VendaModel VendaModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var httpClient = new HttpClient();
            var url = $"http://localhost:5151/api/Vendas/{id}";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();
            VendaModel = JsonConvert.DeserializeObject<VendaModel>(content);

            if (VendaModel == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = $"http://localhost:5151/api/Vendas/{id}";
                    var response = await httpClient.DeleteAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToPage("Vendas");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Erro ao tentar apagar a venda.");
                        return Page();
                    }
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Erro de conexão ao tentar apagar a venda.");
                return Page();
            }
        }
    }
}
