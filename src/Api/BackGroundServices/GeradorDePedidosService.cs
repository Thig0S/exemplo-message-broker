using System.Security.Cryptography;
using ExemploMessageBroker.WebApi.Contracts;
using MassTransit;

namespace ExemploMessageBroker.WebApi.BackGroundServices;

public class GeradorDePedidosService(ILogger<GeradorDePedidosService> logger,
    IBus bus //message bus
) : BackgroundService
{
    private static readonly TimeSpan IntervaloGeracao = TimeSpan.FromSeconds(5);
    private static readonly string[] Clientes =
   [
       "Ana", "Bruno", "Carla", "Daniel", "Fernanda", "Lucas", "Marina", "Rafael"
   ];

    private static readonly string[] Estabelecimentos =
    [
        "Burger House", "Cantina da Vila", "Pizza Express", "Sabor Oriental", "Taco Loco"
    ];

    private static readonly string[] Produtos =
    [
        "Hamburguer artesanal", "Pizza margherita", "Lasanha", "Temaki", "Taco", "Batata frita", "Refrigerante", "Suco"
    ];
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var pedido = CriarPedido();

                await bus.Publish(pedido, stoppingToken);

                logger.LogInformation(
                    "Publicaco pedido {pedido.PedidoId} para {Cliente}. Restaurante: {Restaurante}; Itens: {QuantidadeItens}; Total: {ValorTotal}",
                    pedido.PedidoId,
                    pedido.Cliente,
                    pedido.Estabelecimento,
                    pedido.Itens.Sum(e => e.Quantidade),
                    pedido.ValorTotal
                    );

                await Task.Delay(IntervaloGeracao, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Gerador de pedidos encerrado");
        }

    }
    private static PedidoCriado CriarPedido()
    {
        var itens = Enumerable
            .Range(0, RandomNumberGenerator.GetInt32(1, 4))
            .Select(_ => new ItemPedido(
                Produtos[Random.Shared.Next(Produtos.Length)],
                RandomNumberGenerator.GetInt32(1, 4),
                RandomNumberGenerator.GetInt32(1200, 6501) / 100
            )).ToArray();

        return new PedidoCriado(
            Guid.CreateVersion7(),
            Clientes[RandomNumberGenerator.GetInt32(Clientes.Length)],
            Estabelecimentos[RandomNumberGenerator.GetInt32(Estabelecimentos.Length)],
            itens,
            itens.Sum(item => item.Quantidade * item.PrecoUnitario),
            DateTimeOffset.UtcNow
        );
    }
}