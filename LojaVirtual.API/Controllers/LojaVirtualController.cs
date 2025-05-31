using Microsoft.AspNetCore.Mvc;
using LojaVirtual.API.Context;


namespace LojaVirtual.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LojaVirtualController : Controller
    {
        private readonly DbContextMySql _context;
        public LojaVirtualController(DbContextMySql context)
        {
            _context = context;
        }

        [HttpGet(Name = "BuscaPedidos")]
        public IActionResult buscaPedidos(int idPedido)
        {
           var pedidos = _context.buscaPedidos(idPedido);
           return Ok(pedidos);
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
