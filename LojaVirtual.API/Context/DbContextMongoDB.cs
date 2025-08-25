using Elastic.Apm;
using Elastic.Apm.Api;
using MongoDB.Driver;

namespace LojaVirtual.API.Context
{
    public class DbContextMongoDB
    {
        private readonly ILogger<DbContextMongoDB> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _database;
        public IMongoCollection<PedidoMongo> Pedidos => _database.GetCollection<PedidoMongo>("Pedidos");
      
        public DbContextMongoDB(ILogger<DbContextMongoDB> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            var client = new MongoClient(_configuration["ConnectionStrings:MongoDB"]);
            _database = client.GetDatabase("LojaVirtual_mdb");
        }        

        public PedidoMongo BuscarPedido(int idPedido)
        {
            // Cria um span manual para monitorar a busca no MongoDB
            var span = Agent.Tracer.CurrentTransaction?.StartSpan("MongoDB Find Pedido", ApiConstants.TypeDb, ApiConstants.SubTypeMongoDb, ApiConstants.ActionQuery);
            try
            {
                var filtro = Builders<PedidoMongo>.Filter.Eq(p => p.Pedido.IdPedidos, idPedido);
                _logger.LogInformation("Buscando pedido com ID {IdPedido} no MongoDB", idPedido);
                return Pedidos.Find(filtro).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido com ID {IdPedido} no MongoDB", idPedido);
                span?.CaptureException(ex);
                throw;
            }
            finally
            {
                span?.End();
            }
        }
        public int InserePedidoMongo(PedidoMongo pedidoMongo)
        {
            var span = Agent.Tracer.CurrentTransaction?.StartSpan("MongoDB Insert Pedido", ApiConstants.TypeDb, ApiConstants.SubTypeMongoDb, ApiConstants.ActionExec);
            try
            {
                Pedidos.InsertOne(pedidoMongo);
                _logger.LogInformation("Pedido inserido com ID {IdPedido} no MongoDB", pedidoMongo.Pedido.IdPedidos);
                return pedidoMongo.Pedido.IdPedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir pedido com ID {IdPedido} no MongoDB", pedidoMongo.Pedido.IdPedidos);
                span?.CaptureException(ex);
                throw;
            }
            finally
            {
                span?.End();
            }
        }
    }
}
