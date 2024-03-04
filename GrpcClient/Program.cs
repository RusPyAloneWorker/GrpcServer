using Grpc.Net.Client;
using GrpcClient;

var channel = GrpcChannel.ForAddress("https://localhost:10000/grpc");
var client = new Forecast.ForecastClient(channel);
var data = client.Ask();

// получаем поток сервера
var responseStream = data.ResponseStream;
await data.RequestStream.WriteAsync(new ForecastMessage() { Message = "lol"});
await data.RequestStream.WriteAsync(new ForecastMessage() { Message = "lol"});
await data.RequestStream.WriteAsync(new ForecastMessage() { Message = "lol"});
await data.RequestStream.WriteAsync(new ForecastMessage() { Message = "lol"});
await data.RequestStream.WriteAsync(new ForecastMessage() { Message = "lol"});

while (await responseStream.MoveNext(new CancellationToken()))
{
    var response = responseStream.Current;
    Console.WriteLine($"{DateTime.UtcNow.ToString("HH:mm:ss")} {response.Message}");
}

Console.WriteLine();

// The port number must match the port of the gRPC server.