using System.Text.Json.Serialization;
using Grpc.Core;
using Newtonsoft.Json;

namespace GrpcServer.Services;

public class ForecastService : Forecast.ForecastBase
{
    public override async Task Ask(ForecastMessage request,
        IServerStreamWriter<ForecastReply> responseStream,
        ServerCallContext context)
    {
        CancellationToken token = new CancellationToken();
        while (!context.CancellationToken.IsCancellationRequested)
        {
            
            await responseStream.WriteAsync(new ForecastReply() { Message = "We got your message" }, context.CancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
        }
    }
}
