using System.Text.Json.Serialization;
using Grpc.Core;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GrpcServer.Services;

public class ForecastService : Forecast.ForecastBase
{
    private readonly string URL = "https://api.open-meteo.com/v1/forecast";
    public override async Task Ask(ForecastMessage request,
        IServerStreamWriter<ForecastReply> responseStream,
        ServerCallContext context)
    {
        int count = 0;
        int day = 01;
        int hour = 00;
        string message;
        using var client = new HttpClient();
        while (!context.CancellationToken.IsCancellationRequested && count <= 10)
        {
            var _day = day.ToString().Length == 1 ? $"0{day}" : day.ToString();
            var _hour = hour.ToString().Length == 1 ? $"0{hour}" : hour.ToString();
            var urlWithParams = URL + $"?latitude=52.52&longitude=13.41&" +
                                $"hourly=temperature_2m&" +
                                $"start_hour=2024-01-{_day}T{_hour}:00" +
                                $"&end_hour=2024-01-{_day}T{_hour}:00";
            try
            {
                using var result = await client.GetAsync(urlWithParams, context.CancellationToken);
                var contents = await result.Content.ReadAsStringAsync();
                dynamic weather = JsonConvert.DeserializeObject(contents);
                var date_and_time = weather.hourly.time[0];
                string dummy = (string)(date_and_time);
                var split = dummy.Split("T");
                message = $"погода на {split[0]} {split[1]} {weather.hourly.temperature_2m[0]}C";
            }
            catch (Exception e)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), context.CancellationToken);
                continue;
            }
            
            await responseStream.WriteAsync(new ForecastReply() { Message = message }, context.CancellationToken);

            count++;
            if (hour >= 24)
            {
                day++;
                hour = 0;
            }
            else
            {
                hour += 2;
            }
            await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
        }
    }

    public class Weather
    {
        [JsonProperty("hourly")]
        public Hourly Hourly;
    }

    public class Hourly
    {
        [JsonProperty("time")]
        public string[] Time { get; set; }
        
        [JsonProperty("temperature_2m")]
        public double[] Temperature_2m { get; set; }
    }
}
