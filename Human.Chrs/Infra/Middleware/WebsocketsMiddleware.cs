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
                var adminToken = context.Request.Query["AdminToken"].ToString();
                string userKey = null;

                if (string.IsNullOrEmpty(adminToken))
                {
                    var staffId = int.Parse(context.Request.Query["staffId"].ToString());
                    userKey = staffId.ToString();
                    await _webSocketHandler.AddStaffSocket(staffId, socket);
                }
                else
                {
                    var adminId = int.Parse(context.Request.Query["AdminId"].ToString());
                    if (await _webSocketHandler.VerifyAdminToken(adminToken, adminId))
                    {
                        userKey = adminToken;
                        await _webSocketHandler.AddAdminSocket(adminToken, adminId, socket);
                    }
                    else
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "AdminToken verification failed.", CancellationToken.None);
                    }
                }

                await Receive(socket, userKey, async (result, buffer, key) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        await _webSocketHandler.ReceiveAsync(socket, result, buffer);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        if (string.IsNullOrEmpty(adminToken))
                            await _webSocketHandler.RemoveStaffSocketAsync(int.Parse(key));
                        else
                            await _webSocketHandler.RemoveAdminSocketAsync(key);
                    }
                });
            }
            else
            {
                await _next(context);  // Important! This will allow the request to continue to other middlewares and eventually to your controllers.
            }
        }

        private async Task Receive(WebSocket socket, string userKey, Action<WebSocketReceiveResult, byte[], string> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer, userKey);
            }
        }
    }
}