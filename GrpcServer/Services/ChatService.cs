using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace GrpcServer.Services;

[Authorize]
public class ChatService: Chat.ChatBase
{
    private readonly SessionStorage _storage;

    public ChatService(SessionStorage storage)
    {
        _storage = storage;
    }
    
    public override async Task JoinChat(JoinChatRequest request, IServerStreamWriter<MessageNotification> responseStream, ServerCallContext context)
    {
        foreach (var message in _storage.Messages)
        {
            if (context.CancellationToken.IsCancellationRequested)
                return;
            
            await responseStream.WriteAsync(new MessageNotification() { Message = message }, context.CancellationToken);
        }
        
        var res = _storage.Sessions.TryAdd(Guid.NewGuid(), (responseStream, context.CancellationToken));

        while (res && !context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(4));
        }
    }

    public override async Task<SendMessageResult> SendMessage(Message request, ServerCallContext context)
    {
        foreach (var session in _storage.Sessions)
        {
            if (session.Value.Token.IsCancellationRequested)
            {
                _storage.Sessions.Remove(session.Key, out _);
                continue;
            }

            await session.Value.StreamWriter.WriteAsync(
                new MessageNotification() {Message = request.Message_});
        }

        return new SendMessageResult() { Message = "Ok" };
    }
}