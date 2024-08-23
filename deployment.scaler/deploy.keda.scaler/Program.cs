using deploy.keda.scaler.Extension;
using deploy.keda.scaler.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(webOptions =>
{
    var gRpcPort = builder.Configuration.GetValue<int>("GrpcPort");
    webOptions.ListenAnyIP(gRpcPort, kestrelOptions =>
    {
        kestrelOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddMongoRepo(builder.Configuration, "MongoDB", "ConnectionString");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<KedaScalerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
