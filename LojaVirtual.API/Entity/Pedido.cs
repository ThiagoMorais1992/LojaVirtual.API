using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LojaVirtual.API.Entity
{
    public class Pedido
    {
        public int idpedidos { get; set; }        
        public List<DetalhePedido> detalhes { get; set; }
        public Cliente cliente { get; set; }
        public DateTime data { get; set; }
        public string situacao { get; set; }
    }
}
