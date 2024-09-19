using Microsoft.AspNetCore.Mvc;

namespace PetConecta.Models
{
    public class CompraInputModel
    {
        public int ClienteId { get; set; }
        public int FuncionarioId { get; set; }
        public List<int> ProdutosIds { get; set; }
    }


}