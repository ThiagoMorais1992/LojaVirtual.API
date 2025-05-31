namespace LojaVirtual.API.Entity
{
    public class DetalhePedido
    {
       public Produto produto { get; set; }
        public int quantidade { get; set; }
        public decimal valor_unt { get; set; }
        
    }
}
