var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    Aplicacao = "Exemplo de Message Broker",
    Status = "Gerando pedidos aleatorios",
    Fila = "pedidos-criados",
    RabbitMqUi = "http://localhost:15672"
}));

app.Run();
