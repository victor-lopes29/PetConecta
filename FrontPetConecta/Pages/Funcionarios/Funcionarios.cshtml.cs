using FrontPetConecta.Data; // Adicione esta linha
using FrontPetConecta.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrontPetConecta.Pages.Funcionario
{
    public class FuncionariosModel : PageModel
    {
        public List<FuncionarioModel> FuncionarioList { get; set; } = new List<FuncionarioModel>();

        public FuncionariosModel() { }

        public async Task<IActionResult> OnGetAsync()
        {
            var httpFuncionario = new HttpClient();
            var url = "http://localhost:5151/api/Funcionarios";
            var response = await httpFuncionario.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                FuncionarioList = JsonConvert.DeserializeObject<List<FuncionarioModel>>(content);
            }
            else
            {
                // Tratar o caso em que a requisi��o n�o foi bem-sucedida
                // Pode adicionar mensagens de erro ou redirecionar para uma p�gina de erro
            }

            return Page();
        }
    }
}
