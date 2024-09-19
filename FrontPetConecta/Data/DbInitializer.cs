using FrontPetConecta.Models;

namespace FrontPetConecta.Data
{
    public class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Clientes!.Any()) // Renomeei para "Clientes"
            {
                return;
            }

            var clientes = new ClienteModel[]
            {
            new ClienteModel
            {
                NomeCliente = "Victor",
                EmailCliente = "victor@hotmail.com",
                TelefoneCliente = "(13)991143790",
                EnderecoCliente = "Rua Manaus",
                TipoPet = "Gato"
            },
            };

            context.AddRange(clientes);
            context.SaveChanges();
        }
    }

}
