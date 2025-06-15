using MySql.Data.MySqlClient;
using LojaVirtual.API.Entity;
using Dapper;

namespace LojaVirtual.API.Context
{
    public class DbContextMySql
    {
        private readonly ILogger<DbContextMySql> _logger;
        private readonly IConfiguration _configuration;
        public DbContextMySql(ILogger<DbContextMySql> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public List<Pedido> buscaPedidos(int idPedido)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration["ConnectionStrings:MySql"]))
                {
                    conn.Open();

                    string sql = @"Select 
                        p.idpedidos, p.idcliente, p.data, p.situacao,
                        cli.nome, cli.email, cli.telefone, cli.dtNascimento, 
                        dp.quantidade, dp.valor_unt,
                        prd.idprodutos, prd.nome, prd.valor
                        from detalhe_pedido dp 
                        join pedidos p on (dp.idpedido = p.idpedidos) 
                        join produtos prd on (dp.idproduto = prd.idprodutos) 
                        join clientes cli on (p.idcliente = cli.idclientes)
                        WHERE p.idpedidos = @idPedido";

                    var pedidoDict = new Dictionary<int, Pedido>();

                    var pedidos = conn.Query<Pedido, Cliente, DetalhePedido, Produto, Pedido>(
                        sql,
                        (pedido, cliente, detalhe, produto) =>
                        {
                            if (!pedidoDict.TryGetValue(pedido.idpedidos, out var pedidoEntry))
                            {
                                pedidoEntry = pedido;
                                pedidoEntry.cliente = cliente;
                                pedidoEntry.detalhes = new List<DetalhePedido>();
                                pedidoDict.Add(pedidoEntry.idpedidos, pedidoEntry);
                            }

                            detalhe.produto = produto;
                            pedidoEntry.detalhes.Add(detalhe);

                            return pedidoEntry;
                        },
                        new { idPedido },
                        splitOn: "idpedidos,nome,quantidade,idprodutos"

                    ).Distinct().ToList();

                    _logger.LogInformation("Pedido: {IdPedido} recuperado com sucesso!", idPedido);
                        
                    return pedidos;                   
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar Pedido: {IdPedido}", idPedido);
            }
            return new List<Pedido>();
        }
    }
}

