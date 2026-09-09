
using ExemploMessageBroker.WebApi.BackGroundServices;
using ExemploMessageBroker.WebApi.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("RabbitMq") ??
    throw new InvalidOperationException("A connection string RabbitMq nao foi configurada");

builder.Services.AddMassTransit(config =>
{
    //add o consumer
    config.AddConsumer<PedidoCriadoConsumer>();

    //use o rabbitMq para o transporte de mensagens
    config.UsingRabbitMq((context, rabbitMq) =>
    {
        rabbitMq.Host(new Uri(connectionString));

        rabbitMq.ReceiveEndpoint("pedidos-criados", endpoint =>
        {
            // Apenas uma mensagem seja entrege e processada por vez
            endpoint.PrefetchCount = 1;
            endpoint.ConcurrentMessageLimit = 1;

            endpoint.ConfigureConsumer<PedidoCriadoConsumer>(context);
        });
    });
});

//Impede que o produtor do MassTransit inicie antes que os endpoints estejam disponiveis
builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
    options.StartTimeout = TimeSpan.FromSeconds(30);
});

//executa o servico em paralelo com a API
builder.Services.AddHostedService<GeradorDePedidosService>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    Aplicacao = "Exemplo de Message Broker",
    Status = "Gerando pedidos aleatorios",
    Fila = "pedidos-criados",
    RabbitMqUi = "http://localhost:15672"
}));

app.Run();
