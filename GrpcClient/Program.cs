using Grpc.Net.Client;
using GrpcClient;

var channel = GrpcChannel.ForAddress("http://localhost:5047");
var client = new Forecast.ForecastClient(channel);
var data = client.Ask(
    new ForecastMessage() );

// получаем поток сервера
var responseStream = data.ResponseStream;

while (await responseStream.MoveNext(new CancellationToken()))
{
 
    var response = responseStream.Current;
    Console.WriteLine($"{DateTime.UtcNow.ToString("HH:mm:ss")} {response.Message}");
}

Console.WriteLine();

// The port number must match the port of the gRPC server.