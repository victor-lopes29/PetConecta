using PetConecta.Data;
using PetConecta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace PetConecta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var produtos = _context.Produtos.ToList();
            return Ok(produtos);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(x => x.ProdutoId == id);

            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Produto produto)
        {
            _context.Produtos.Add(produto);
            _context.SaveChanges();
            return Created($"/api/produtos/{produto.ProdutoId}", produto);
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] Produto produto)
        {
            if (produto == null)
            {
                return BadRequest("Produto não pode ser nulo.");
            }

            var existingProduto = _context.Produtos.FirstOrDefault(x => x.ProdutoId == id);

            if (existingProduto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            // Atualiza apenas as propriedades que foram modificadas
            existingProduto.Nome = produto.Nome;
            existingProduto.Preco = produto.Preco;
            existingProduto.QuantidadeEmEstoque = produto.QuantidadeEmEstoque;
            existingProduto.Categoria = produto.Categoria;
            existingProduto.Descricao = produto.Descricao;

            try
            {
                _context.SaveChanges();
                return Ok(existingProduto);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409, "Conflito de concorrência ao atualizar o produto.");
            }
        }



        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(x => x.ProdutoId == id);

            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return Ok(produto);
        }
    }
}
