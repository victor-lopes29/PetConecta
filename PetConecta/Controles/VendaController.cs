﻿using PetConecta.Data;
using PetConecta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PetConecta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var vendas = _context.Venda
                    .Include(v => v.Funcionario)
                    .Include(v => v.ProdutosVendidos)
                    .Include(v => v.Clientes) 
                    .ToList();

                var jsonOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                };

                var vendasComClienteId = vendas.Select(v =>
                {
                    v.ClienteId = v.Clientes?.IdCliente ?? 0;
                    return v;
                });
                
                var json = JsonSerializer.Serialize(vendasComClienteId, jsonOptions);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"Erro ao recuperar as vendas: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var venda = _context.Venda
                    .Include(v => v.ProdutosVendidos)
                    .FirstOrDefault(v => v.VendaId == id);

                if (venda != null)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                        // Outras opções, se necessário
                    };

                    var json = JsonSerializer.Serialize(venda, jsonOptions);

                    return Ok(json);
                }
                else
                {
                    return NotFound(new { Mensagem = $"Venda com ID {id} não encontrada." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"Erro ao recuperar a venda: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] VendaInputModel vendaInputModel)
        {
            try
            {
                var novaVenda = CriarNovaVenda(vendaInputModel);

                // Salvar a venda no banco diretamente aqui
                _context.Venda.Add(novaVenda);
                _context.SaveChanges();

                // Restante do seu código...

                return Ok(new { VendaId = novaVenda.VendaId, Mensagem = "Venda criada com sucesso" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return BadRequest(new { Mensagem = $"Erro ao criar a venda: {ex.Message}" });
            }
        }

        private Venda CriarNovaVenda(VendaInputModel vendaInputModel)
        {
            var funcionario = _context.Funcionario.Find(vendaInputModel.FuncionarioId);

            if (funcionario == null)
            {
                throw new Exception($"Funcionário com ID {vendaInputModel.FuncionarioId} não encontrado.");
            }

            var novaVenda = new Venda
            {
                DataVenda = DateTime.Now,
                ProdutosVendidos = new List<ProdutoVendido>(),
                ClienteId = vendaInputModel.ClienteId,
                FuncionarioId = vendaInputModel.FuncionarioId,
                Funcionario = funcionario
            };

            foreach (var produtoVendidoInput in vendaInputModel.ProdutosVendidos)
            {
                var produto = _context.Produtos.Find(produtoVendidoInput.ProdutoId);

                if (produto != null)
                {
                    if (produto.QuantidadeEmEstoque >= produtoVendidoInput.Quantidade)
                    {
                        var produtoVendido = new ProdutoVendido
                        {
                            Produto = produto,
                            Quantidade = produtoVendidoInput.Quantidade,
                            PrecoUnitario = produto.Preco,
                            FuncionarioId = vendaInputModel.FuncionarioId
                        };

                        novaVenda.ProdutosVendidos.Add(produtoVendido);

                        // Atualizar a quantidade em estoque
                        produto.QuantidadeEmEstoque -= produtoVendidoInput.Quantidade;
                        produto.QuantidadeVendida += produtoVendidoInput.Quantidade;
                    }
                    else
                    {
                        // Lide com a situação em que a quantidade em estoque é insuficiente
                        throw new Exception($"Quantidade em estoque insuficiente para o produto com ID {produtoVendidoInput.ProdutoId}.");
                    }
                }
                else
                {
                    // Lide com a situação em que o produto não é encontrado
                    throw new Exception($"Produto com ID {produtoVendidoInput.ProdutoId} não encontrado.");
                }
            }

            novaVenda.CalcularTotalVenda();

            return novaVenda;
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] VendaInputModel vendaInputModel)
        {
            try
            {
                // Buscar a venda existente no banco de dados
                var vendaExistente = _context.Venda
                    .Include(v => v.ProdutosVendidos)
                    .FirstOrDefault(v => v.VendaId == id);

                if (vendaExistente == null)
                {
                    return NotFound(new { Mensagem = $"Venda com ID {id} não encontrada." });
                }

                // Atualizar as informações da venda
                var funcionario = _context.Funcionario.Find(vendaInputModel.FuncionarioId);
                if (funcionario == null)
                {
                    return BadRequest(new { Mensagem = $"Funcionário com ID {vendaInputModel.FuncionarioId} não encontrado." });
                }

                vendaExistente.FuncionarioId = vendaInputModel.FuncionarioId;
                vendaExistente.Funcionario = funcionario;
                vendaExistente.ClienteId = vendaInputModel.ClienteId;

                // Remover os produtos vendidos antigos
                _context.ProdutoVendido.RemoveRange(vendaExistente.ProdutosVendidos);
                vendaExistente.ProdutosVendidos.Clear();

                // Adicionar os novos produtos vendidos
                foreach (var produtoVendidoInput in vendaInputModel.ProdutosVendidos)
                {
                    var produto = _context.Produtos.Find(produtoVendidoInput.ProdutoId);
                    if (produto != null)
                    {
                        if (produto.QuantidadeEmEstoque >= produtoVendidoInput.Quantidade)
                        {
                            var produtoVendido = new ProdutoVendido
                            {
                                Produto = produto,
                                Quantidade = produtoVendidoInput.Quantidade,
                                PrecoUnitario = produto.Preco,
                                FuncionarioId = vendaInputModel.FuncionarioId
                            };

                            vendaExistente.ProdutosVendidos.Add(produtoVendido);

                            // Atualizar a quantidade em estoque
                            produto.QuantidadeEmEstoque -= produtoVendidoInput.Quantidade;
                            produto.QuantidadeVendida += produtoVendidoInput.Quantidade;
                        }
                        else
                        {
                            return BadRequest(new { Mensagem = $"Quantidade insuficiente em estoque para o produto com ID {produtoVendidoInput.ProdutoId}." });
                        }
                    }
                    else
                    {
                        return BadRequest(new { Mensagem = $"Produto com ID {produtoVendidoInput.ProdutoId} não encontrado." });
                    }
                }

                // Recalcular o total da venda
                vendaExistente.CalcularTotalVenda();

                // Salvar as alterações no banco de dados
                _context.SaveChanges();

                return Ok(new { Mensagem = "Venda atualizada com sucesso.", VendaId = vendaExistente.VendaId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"Erro ao atualizar a venda: {ex.Message}" });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var venda = _context.Venda
                    .Include(v => v.ProdutosVendidos)
                    .FirstOrDefault(v => v.VendaId == id);

                if (venda == null)
                {
                    return NotFound(new { Mensagem = $"Venda com ID {id} não encontrada." });
                }
                
                _context.Venda.Remove(venda);
                _context.SaveChanges();

                return Ok(new { Mensagem = "Venda excluída com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = $"Erro ao excluir a venda: {ex.Message}" });
            }
        }


    }
}