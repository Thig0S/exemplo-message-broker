# Exemplo de Message Broker

Desenvolvido durante o curso Fullstack da [Academia do Programador 2026](https://www.academiadoprogramador.net).

Exemplo didático de produtor e consumidor de mensagens com .NET, MassTransit e RabbitMQ.

O projeto usa MassTransit 8.5.10, a última versão 8, para que o protótipo possa ser executado sem uma chave de licença exigida pelo MassTransit 9.

Enquanto a aplicação está em execução:

- `GeradorPedidosService` cria e publica um pedido aleatório a cada 6 segundos.
- O MassTransit entrega os eventos para a fila durável `pedidos-criados`.
- `PedidoCriadoConsumer` processa uma mensagem por vez, levando 6 segundos.
- Durante o processamento, a mensagem permanece visível como não confirmada na UI do RabbitMQ.

## Pré-requisitos

- .NET SDK 10
- RabbitMQ acessível em `localhost:5672`
- RabbitMQ Management acessível em `http://localhost:15672`
- Usuário e senha `guest`

## Executar

Na raiz do repositório:

```bash
dotnet run --project MessageBrokerExample.Api
```

Os logs mostram cada pedido publicado, recebido e processado.

## Visualizar no RabbitMQ

1. Acesse `http://localhost:15672` e entre com `guest` / `guest`.
2. Abra **Queues and Streams**.
3. Selecione a fila `pedidos-criados`.
4. Observe os contadores **Ready**, **Unacked** e **Total**, além das taxas de mensagens.
5. Para inspecionar um payload, abra **Get messages**, use **Ack Mode** igual a `Nack message requeue true`, informe `1` em **Messages** e clique em **Get Message(s)**.

O modo `Nack message requeue true` devolve a mensagem para a fila depois da inspeção.

## Configuração

Os valores ficam em `MessageBrokerExample.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "RabbitMq": "amqp://guest:guest@localhost:5674/"
  }
}
```

Qualquer valor pode ser sobrescrito por variável de ambiente usando dois sublinhados entre as seções. Por exemplo:

```bash
ConnectionStrings__RabbitMq=amqp://app:senha@meu-rabbitmq:5672/ dotnet run --project MessageBrokerExample.Api
```

Os tempos da simulação ficam nas constantes `IntervaloGeracao`, em `GeradorPedidosService`, e `TempoProcessamento`, em `PedidoCriadoConsumer`.
