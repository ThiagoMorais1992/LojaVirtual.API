using MySql.Data.MySqlClient;
using LojaVirtual.API.Entity;
using Dapper;

namespace LojaVirtual.API.Context
{
    public class DbContextMySql
    {
        private readonly IConfiguration _configuration;
        public DbContextMySql(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private MySqlConnection criaConexao()
        {
            MySqlConnection conn = new MySqlConnection(_configuration["ConnectionStrings:MySql"]);

            try
            {
                conn.Open();
                Console.WriteLine("Conexão aberta com sucesso!");
                return conn;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao abrir a conexão: " + ex.Message);
                return null;
            }
        }

        private void fechaConexao(MySqlConnection conn)
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    conn.Close();
                    Console.WriteLine("Conexão fechada com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao fechar a conexão: " + ex.Message);
                }
            }
        }

        public Object executaComando(string sql)
        {
            using (MySqlConnection conn = criaConexao())
            {
                if (conn != null)
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        try
                        {
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Comando executado com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro ao executar o comando: " + ex.Message);
                        }
                    }
                    fechaConexao(conn);
                }
            }
            return new Pedido(); // Retorna um objeto Pedido vazio, pode ser ajustado conforme a necessidade
        }

        public List<Pedido> buscaPedidos(int idPedido)
        {
            try
            {
                using (MySqlConnection conn = criaConexao())
                {
                    if (conn != null)
                    {
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

                        Console.WriteLine($"Pedido: {idPedido} recuperado com sucesso!");
                        
                        fechaConexao(conn);                        
                        
                        return pedidos;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar Pedido: {idPedido} Erro: {ex.Message}");
            }
            return new List<Pedido>();
        }
    }
}

