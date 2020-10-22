using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace astorWorkRealTimeService
{
    public class ChatService
    {
        public ChatService(RequestDelegate requestDelegate)
        {
            this.requestDelegate = requestDelegate;     
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await requestDelegate.Invoke(context);
                return;
            }

            var token = context.RequestAborted;
            var socket = await context.WebSockets.AcceptWebSocketAsync();

            var guid = Guid.NewGuid().ToString();
            sockets.TryAdd(guid, socket);

            while (!token.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                var message = new ArraySegment<byte>(new byte[4096]);
                var receivedMessage = new List<byte>();
                do
                {
                    token.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(message, token);
                    var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                    receivedMessage.AddRange(messageBytes);
                } while (!result.EndOfMessage);
                

                if (receivedMessage.Count == 0)
                {
                    if (socket.State != WebSocketState.Open)
                        break;

                    continue;
                }

                var messageToSent = receivedMessage.ToArray();
                var segmnet = new ArraySegment<byte>(messageToSent);
                foreach (var s in sockets.Where(p => p.Key != guid && p.Value.State == WebSocketState.Open))
                    await s.Value.SendAsync(segmnet, result.MessageType, true, token);

                await SendMessageAsync(socket, "SUCCESS", token);
            }

            sockets.TryRemove(guid, out WebSocket redundantSocket);

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Session ended", token);
            socket.Dispose();
        }

        async Task<string> GetMessageAsync(WebSocket socket, CancellationToken token)
        {
            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>(new byte[4096]);
            string receivedMessage = string.Empty;

            do
            {
                token.ThrowIfCancellationRequested();

                result = await socket.ReceiveAsync(message, token);
                var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                receivedMessage = Encoding.UTF8.GetString(messageBytes);

            } while (!result.EndOfMessage);

            if (result.MessageType != WebSocketMessageType.Text)
                return null;

            return receivedMessage;
        }

        Task SendMessageAsync(WebSocket socket, string message, CancellationToken token)
        {
            var byteMessage = Encoding.UTF8.GetBytes(message);
            var segmnet = new ArraySegment<byte>(byteMessage);

            return socket.SendAsync(segmnet, WebSocketMessageType.Text, true, token);
        }

        Task SendMessageAsync(WebSocket socket, byte[] byteMessage, CancellationToken token)
        {
            var segmnet = new ArraySegment<byte>(byteMessage);

            return socket.SendAsync(segmnet, WebSocketMessageType.Binary, true, token);
        }

        readonly RequestDelegate requestDelegate;
        ConcurrentDictionary<string, WebSocket> sockets = new ConcurrentDictionary<string, WebSocket>();
    }
}
