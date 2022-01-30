using SampleGrpc.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<ProtoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    var protoService = endpoints.ServiceProvider.GetRequiredService<ProtoService>();
    endpoints.MapGet("/protos", async context =>
    {
        await context.Response.WriteAsync(await protoService.GetAllAsync());
    });
    endpoints.MapGet("/Protos/v{version:int}/{protoName}", async context =>
    {
        var version = int.Parse((string)context.Request.RouteValues["version"]);
        var protoName = (string)context.Request.RouteValues["protoName"];

        var filePath = protoService.Get(version, protoName);

        if (filePath != null)
        {
            await context.Response.SendFileAsync(filePath);
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    });
});
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();