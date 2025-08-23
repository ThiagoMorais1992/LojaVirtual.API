using MySql.Data.MySqlClient;
using LojaVirtual.API.Entity;
using Dapper;
using Elastic.Apm;
using Elastic.Apm.Api;

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

                    var span = Agent.Tracer.CurrentTransaction?.StartSpan("MySQL Find Pedido", ApiConstants.TypeDb, ApiConstants.SubtypeMySql, ApiConstants.ActionQuery);
                    
                    try
                    {
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
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao buscar pedido com ID {IdPedido} no MySQL", idPedido);
                        span?.CaptureException(ex);
                        throw;
                    }
                    finally
                    {
                        span?.End();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar Pedido: {IdPedido}", idPedido);
            }
            return new List<Pedido>();
        }

        public List<Pedido> buscaPedidos2(int idPedido)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration["ConnectionStrings:MySql"]))
                {
                    conn.Open();

                    string sqlPedidos = @"SELECT idpedidos, idcliente, data, situacao
                                    FROM lojavirtual_mdb.pedidos
                                    WHERE idpedidos = @idPedido";
                    string sqlDetalhePed = @"SELECT iddetalhe_pedido, idpedido, idproduto, quantidade, valor_unt
                                    FROM lojavirtual_mdb.detalhe_pedido
                                    WHERE idpedido = @idPedido";
                    string sqlProdutos = @"SELECT idprodutos, nome, valor
                                    FROM lojavirtual_mdb.produtos
                                    WHERE idprodutos = @idprodutos";
                    string sqlClientes = @"SELECT idClientes, nome, email, telefone, dtNascimento
                                    FROM lojavirtual_mdb.clientes
                                    WHERE idClientes = @idClientes";

                    var span = Agent.Tracer.CurrentTransaction?.StartSpan("MySQL Find Pedido", ApiConstants.TypeDb, ApiConstants.SubtypeMySql, ApiConstants.ActionQuery);

                    try
                    {
                        // Consulta o pedido
                        var pedido = conn.QueryFirstOrDefault<Pedido>(sqlPedidos, new { idPedido });
                        if (pedido == null)
                            return new List<Pedido>();

                        // Consulta o cliente
                        var cliente = conn.QueryFirstOrDefault<Cliente>(sqlClientes, new { idClientes = pedido.idcliente });
                        pedido.cliente = cliente;

                        // Consulta os detalhes do pedido
                        var detalhes = conn.Query(sqlDetalhePed, new { idPedido }).ToList();

                        pedido.detalhes = new List<DetalhePedido>();
                        foreach (var detalhe in detalhes)
                        {
                            var detalhePedido = new DetalhePedido
                            {
                                quantidade = detalhe.quantidade,
                                valor_unt = detalhe.valor_unt
                            };

                            // Consulta o produto de cada detalhe
                            var produto = conn.QueryFirstOrDefault<Produto>(sqlProdutos, new { idprodutos = detalhe.idproduto });
                            detalhePedido.produto = produto;

                            pedido.detalhes.Add(detalhePedido);
                        }

                        _logger.LogInformation("Pedido: {IdPedido} recuperado com sucesso!", idPedido);

                        return new List<Pedido> { pedido };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao buscar pedido com ID {IdPedido} no MySQL", idPedido);
                        span?.CaptureException(ex);
                        throw;
                    }
                    finally
                    {
                        span?.End();
                    }
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

