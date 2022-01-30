using Grpc.Core;
using SampleGrpc;
using System.Text.Json;

namespace SampleGrpc.Services
{
    public class ProtoService
    {
        private readonly string _baseDirectory;
        private readonly Dictionary<string, IEnumerable<string>> _protosByVersion;

        public ProtoService(IWebHostEnvironment webHost)
        {
            _baseDirectory = webHost.ContentRootPath;
            _protosByVersion = Get(_baseDirectory);
        }

        public async Task<string> GetAllAsync()
        {
            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(stream, _protosByVersion);
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
        }

        public string Get(int version, string protoName)
        {
            var filePath = $"Protos/v{version}/{protoName}";
            var exist = File.Exists($"{_baseDirectory}/{filePath}");

            return exist ? filePath : null;
        }

        private Dictionary<string, IEnumerable<string>> Get(string baseDirectory) =>

            Directory.GetDirectories($"Protos")
            .Select(x => new { version = x, protos = Directory.GetFiles(x).Select(Path.GetFileName) })
            .ToDictionary(o => Path.GetRelativePath("protos", o.version), o => o.protos);
    }
}