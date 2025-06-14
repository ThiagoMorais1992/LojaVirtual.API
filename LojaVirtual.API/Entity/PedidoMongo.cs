using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

public class PedidoMongo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("pedido")]
    public PedidoM Pedido { get; set; }

    [BsonElement("cliente")]
    public ClienteM Cliente { get; set; }

    [BsonElement("detalhes_pedido")]
    public List<DetalhePedidoM> DetalhesPedido { get; set; }
}

public class PedidoM
{
    [BsonElement("idpedidos")]
    public int IdPedidos { get; set; }

    [BsonElement("idcliente")]
    public int IdCliente { get; set; }

    [BsonElement("data")]
    public DateTime Data { get; set; }

    [BsonElement("situacao")]
    public string Situacao { get; set; }
}

public class ClienteM
{
    [BsonElement("idclientes")]
    public int IdClientes { get; set; }

    [BsonElement("nome")]
    public string Nome { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("telefone")]
    public string Telefone { get; set; }

    [BsonElement("dtNascimento")]
    public DateTime DtNascimento { get; set; }
}

public class DetalhePedidoM
{
    [BsonElement("idprodutos")]
    public int IdProdutos { get; set; }

    [BsonElement("nome_produto")]
    public string NomeProduto { get; set; }

    [BsonElement("valor_produto")]
    public decimal ValorProduto { get; set; }

    [BsonElement("quantidade")]
    public int Quantidade { get; set; }

    [BsonElement("valor_unt")]
    public decimal ValorUnt { get; set; }
}
