using FrontPetConecta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FrontPetConecta.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrontPetConecta.Pages.Vendas
{
    public class CreateModel : PageModel
    {
        private readonly string apiUrl = "http://localhost:5151/";

        public List<VendaInputModel> VendaList { get; set; } = new();
        public List<ProdutoModel> ProdutoList { get; set; } = new();
        public List<ClienteModel> ClienteList { get; set; } = new();
        public List<FuncionarioModel> FuncionarioList { get; set; } = new();

        public CreateModel() { }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDataAsync();
            return Page();
        }

        [BindProperty]
        public VendaInputModel VendaModel { get; set; }

        // Adicionado para preencher a lista de produtos no dropdown
        [BindProperty]
        public int ProdutoId { get; set; }
        
        [BindProperty]
        public int ClienteId { get; set; }

        [BindProperty]
        public int FuncionarioId { get; set; }

        [BindProperty]
        public int Quantidade { get; set; }

        // Adicionado para preencher a lista de produtos no dropdown
        public SelectList ProdutoSelectList { get; set; }
        public SelectList ClienteSelectList { get; set; }
        public SelectList FuncionarioSelectList { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Cria um novo ProdutoVendidoModel a partir dos dados do formul�rio
                var produtoVendido = new ProdutoVendidoModel
                {
                    ProdutoId = ProdutoId,
                    Quantidade = Quantidade
                };

                // Adiciona o produto vendido � lista existente
                VendaModel.ProdutosVendidos.Add(produtoVendido);

                try
                {
                    var httpClient = new HttpClient();
                    var url = "http://localhost:5151/api/Vendas";
                    var serializedCliente = JsonConvert.SerializeObject(VendaModel);
                    var content = new StringContent(serializedCliente, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Voc� pode redirecionar para a p�gina de listagem ou fazer algo mais
                        return RedirectToPage("Vendas");
                    }
                    else
                    {
                        return Page();
                    }
                }
                catch (HttpRequestException)
                {
                    return Page();
                }
            }

            await LoadDataAsync();
            return Page();
        }

        private async Task LoadDataAsync()
        {
            var httpClient = new HttpClient();

            // Carrega a lista de vendas
            var vendaResponse = await httpClient.GetAsync(apiUrl);
            var vendaContent = await vendaResponse.Content.ReadAsStringAsync();
            VendaList = JsonConvert.DeserializeObject<List<VendaInputModel>>(vendaContent);

            // Carrega a lista de produtos para preencher o dropdown
            var produtoResponse = await httpClient.GetAsync(apiUrl + "api/Produtos");
            var produtoContent = await produtoResponse.Content.ReadAsStringAsync();
            ProdutoList = JsonConvert.DeserializeObject<List<ProdutoModel>>(produtoContent);
            ProdutoSelectList = new SelectList(ProdutoList, nameof(ProdutoModel.ProdutoId), nameof(ProdutoModel.Nome));

            var clienteResponse = await httpClient.GetAsync(apiUrl + "api/Clientes");
            var clienteContent = await clienteResponse.Content.ReadAsStringAsync();
            ClienteList = JsonConvert.DeserializeObject<List<ClienteModel>>(clienteContent);
            ClienteSelectList = new SelectList(ClienteList, nameof(ClienteModel.IdCliente), nameof(ClienteModel.NomeCliente));

            var funcionarioResponse = await httpClient.GetAsync(apiUrl + "api/Funcionarios");
            var funcionarioContent = await funcionarioResponse.Content.ReadAsStringAsync();
            FuncionarioList = JsonConvert.DeserializeObject<List<FuncionarioModel>>(funcionarioContent);
            FuncionarioSelectList = new SelectList(FuncionarioList, nameof(FuncionarioModel.IdFuncionario), nameof(FuncionarioModel.IdFuncionario));
            // Preenche o SelectList para o dropdown de produtos
        }
    }
}