using Microsoft.AspNetCore.Mvc;
using LojaVirtual.API.Context;

namespace LojaVirtual.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LojaVirtualController : Controller
    {
        private readonly ILogger<LojaVirtualController> _logger;
        private readonly DbContextMySql _context;
        private readonly DbContextMongoDB _contextMongoDB;
        public LojaVirtualController(ILogger<LojaVirtualController> logger, DbContextMySql context, DbContextMongoDB contextMongoDB)
        {
            _logger = logger;
            _context = context;
            _contextMongoDB = contextMongoDB;
        }

        [HttpGet("buscaPedidos/{idPedido}", Name = "BuscaPedidos")]
        public IActionResult buscaPedidos(int idPedido)
        {
            var pedidos = _context.buscaPedidos(idPedido);
            return Ok(pedidos);
        }
        
        [HttpGet("buscaPedidos2/{idPedido}", Name = "BuscaPedidos2")]
        public IActionResult buscaPedidos2(int idPedido)
        {
            var pedidos = _context.buscaPedidos2(idPedido);
            return Ok(pedidos);
        }        

        [HttpPost("InserirPedido", Name = "InserirPedido")]
        public IActionResult InserirPedido([FromBody] Entity.Pedido pedido)
        {
            var idPedido = _context.inserePedido(pedido);
            return Ok($"Pedido {idPedido} inserido com sucesso!");
        }

        [HttpPut("AtualizaPedido/{idPedido}", Name = "AtualizaPedido")]
        public IActionResult AtualizaPedido(int idPedido, [FromBody] string situacao)
        {
            var linhasAfetadas = _context.atualizaPedido(idPedido, situacao);
            return Ok($"Pedido {idPedido} atualizado com sucesso! Linhas afetadas: {linhasAfetadas}");
        }

        [HttpGet("buscaPedidosMongo/{idPedido}", Name = "BuscaPedidosMongo")]
        public IActionResult buscaPedidosMongo(int idPedido)
        {
            var pedidos = _contextMongoDB.BuscarPedido(idPedido);
            return Ok(pedidos);
        }

        [HttpPost("InserirPedidoMongo", Name = "InserirPedidoMongo")]
        public IActionResult InserirPedidoMongo([FromBody] PedidoMongo pedido)
        {
            var idPedido = _contextMongoDB.InserePedidoMongo(pedido);
            return Ok($"Pedido {idPedido} inserido com sucesso!");
        }

        [HttpPut("AtualizaPedidoMongo/{idPedido}", Name = "AtualizaPedidoMongo")]
        public IActionResult AtualizaPedidoMongo(int idPedido, [FromBody] string situacao)
        {
            var linhasAfetadas = _contextMongoDB.AtualizaPedidoMongo(idPedido, situacao);
            return Ok($"Pedido {idPedido} atualizado com sucesso no MongoDB! Linhas afetadas: {linhasAfetadas}");
        }

        [HttpGet("healthcheck", Name = "HealthCheck")]
        public IActionResult HealthCheck()
        {
            return Ok("A aplicação está rodando e girando e rodando");
        }
    }
}
