
using ExemploMessageBroker.WebApi.BackGroundServices;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

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
