using System.Collections.Concurrent;
using Grpc.Core;

namespace GrpcServer.Services;

public class SessionStorage
{
    public List<string> Messages { get; set; } = new () {"Hi there! How are you?", "I'm fine. And you?"};

    public ConcurrentDictionary<Guid, (IServerStreamWriter<MessageNotification> StreamWriter, CancellationToken Token)>
        Sessions { get; set; } = new ();
}