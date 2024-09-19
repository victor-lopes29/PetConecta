using PetConecta.Models;

namespace FrontPetConecta.Models
{
    public class VendaModel
    {
       
            public int VendaId { get; set; }
            public DateTime DataVenda { get; set; }
            public decimal TotalVenda { get; set; }

            public ICollection<ProdutoVendidoModel> ProdutosVendidos { get; set; } = new List<ProdutoVendidoModel>();

            public FuncionarioModel Funcionario { get; set; }
            public int FuncionarioId { get; set; }
            public ClienteModel Clientes { get; set; }
            public int ClienteId { get; set; }
            public string NomeCliente { get; set; }
            public string NomeFuncionario { get; set; }

            public void CalcularTotalVenda()
            {
                if (ProdutosVendidos != null && ProdutosVendidos.Any())
                {
                    TotalVenda = ProdutosVendidos.Sum(produtoVendido => produtoVendido.PrecoUnitario * produtoVendido.Quantidade);
                }
                else
                {
                    TotalVenda = 0;
                }
            }
        }
    }

