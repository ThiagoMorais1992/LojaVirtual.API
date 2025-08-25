namespace LojaVirtual.API.Entity
{
    public class DetalhePedido
    {
        public int idPedido { get; set; }
        public Produto produto { get; set; }
        public int quantidade { get; set; }
        public decimal valor_unt { get; set; }
        
    }
}
