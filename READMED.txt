Esse é um projeto de faculdade desenvolvido para a a matéria de Tecnologia e Desenvolvimento de Sistemas.

A proposta do projeto era desenvolver um sistema de gerenciamento para um pet shop, um aplicativo que permite ao usuário cadastrar clientes, funcionários e produtos, além do registro de compra e venda associada ao cliente, e o valor total de vendas do dia.

--------------------------------------------------

O projeto não está 100% finalizado e está com alguns problemas na parte do front. O principal está na página de "Vendas", quando se tem mais de 1 venda cadastrada no banco. Os dados que são puxados em um JSON vem serializado, no front há uma conversão de JSON deserializando os dados para alocar eles dentro de arrays de um list, mas ele aloca apenas os dados da primeira venda, os demais arrays acabam ficando com o valor nulo, causando o erro no front.

---------------------------------------------------

Como usar:

1 - Instale o pacote SDK do .Net versão 8.
2 - Instale os pacotes NuGet do Provedor de Banco de Dados (Entity Framework Core).
3 - Clone este repositório.
4 - Execute primeiro a API (PetConecta) e depois o front (FrontPetConecta).
