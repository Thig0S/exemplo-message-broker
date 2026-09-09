namespace ExemploMessageBroker.WebApi.Contracts;

public sealed record ItemPedido(
    string Nome,
    int Quantidade,
    decimal PrecoUnitario
);
public sealed record PedidoCriado(
    Guid PedidoId,
    string Cliente,
    string Estabelecimento,
    IReadOnlyCollection<ItemPedido> Itens,
    decimal ValorTotal,
    DateTimeOffset CriadoEmUtc
);