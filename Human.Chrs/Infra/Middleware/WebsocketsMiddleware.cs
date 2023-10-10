using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Human.Chrs.Domain.Websocket;

namespace Human.Chrs.Infra.Middleware
{
    public class WebSocketsMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler;

        public WebSocketsMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var socket = await context.WebSockets.AcceptWebSocketAsync();
                var staffId = int.Parse(context.Request.Query["staffId"].ToString());

                await _webSocketHandler.AddSocket(staffId, socket);

                await Receive(socket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        await _webSocketHandler.ReceiveAsync(socket, result, buffer);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocketHandler.RemoveSocketAsync(staffId);
                    }
                });
            }
            else
            {
                await _next(context);  // Important! This will allow the request to continue to other middlewares and eventually to your controllers.
            }
        }


        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }
    }
}
