using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SampleGrpc;

namespace SampleGrpc.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task GetTable(Empty _, IServerStreamWriter<TableRespnse> responseStream, ServerCallContext context)
        {
            var rnd = new Random();
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
                var data = new TableRespnse() { Num = rnd.Next(), Timestamp = Timestamp.FromDateTime(DateTime.UtcNow) };
                await responseStream.WriteAsync(data);
            }
        }
    }
}