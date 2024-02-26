using Grpc.Core;

namespace GrpcServer.Services;

public class GreeterService : Calc.CalcBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<CalcReply> Ask(CalcRequest request, ServerCallContext context)
    {
        var answer = request.Op switch
        {
            "+" => request.Num1 + request.Num2,
            "-" => request.Num1 - request.Num2,
            "/" => request.Num1 / request.Num2,
            "*" => request.Num1 * request.Num2,
            _ => throw new Exception()
        };
        return Task.FromResult(new CalcReply()
        {
            Message = answer
        });
    }
}