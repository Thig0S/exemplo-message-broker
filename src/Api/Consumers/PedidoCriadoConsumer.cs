using ExemploMessageBroker.WebApi.Contracts;
using MassTransit;

namespace ExemploMessageBroker.WebApi.Consumers;

public class PedidoCriadoConsumer(
    ILogger<PedidoCriadoConsumer> logger
    ) : IConsumer<PedidoCriado>
{
    private static readonly TimeSpan TempoProcessamento = TimeSpan.FromSeconds(6);
    public async Task Consume(ConsumeContext<PedidoCriado> context)
    {
        PedidoCriado pedido = context.Message;

        logger.LogInformation(
            "Recebido pedido {PedidoId} de {Cliente}. Processamento Iniciado",
            pedido.PedidoId,
            pedido.Cliente
        );

        await Task.Delay(TempoProcessamento, context.CancellationToken);

        logger.LogInformation(
            "PEDIDO {PedidoId} processado. Estabelecimento: {Estabelecimento}; Total {ValorTotal:C2}",
            pedido.PedidoId,
            pedido.Estabelecimento,
            pedido.ValorTotal
        );

        //ao finalizar sem exceção, o massTransmit finamente confirmar (ack) a mensagem no rabbit

    }
}