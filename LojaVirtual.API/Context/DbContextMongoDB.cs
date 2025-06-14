using LojaVirtual.API.Entity;
using MongoDB.Driver;

namespace LojaVirtual.API.Context
{
    public class DbContextMongoDB
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _database;
        public DbContextMongoDB(IConfiguration configuration)
        {
            _configuration = configuration;
            var client = new MongoClient(_configuration["ConnectionStrings:MongoDB"]);
            _database = client.GetDatabase("LojaVirtual_mdb"); // Substitua pelo nome do seu banco
        }

        public IMongoCollection<PedidoMongo> Pedidos => _database.GetCollection<PedidoMongo>("Pedidos");

        public PedidoMongo BuscarPedido(int idPedido)
        {
            var filtro = Builders<PedidoMongo>.Filter.Eq(p => p.Pedido.IdPedidos, idPedido);
            return Pedidos.Find(filtro).FirstOrDefault();
        }
    }
}
