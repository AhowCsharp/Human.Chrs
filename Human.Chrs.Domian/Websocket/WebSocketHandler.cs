using Human.Chrs.Domain.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Human.Chrs.Domain.Websocket
{
    public class WebSocketHandler
    {
        private static ConcurrentDictionary<int, WebSocket> _webSockets = new ConcurrentDictionary<int, WebSocket>();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WebSocketHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task AddSocket(int staffId, WebSocket socket)
        {
            if (_webSockets.TryAdd(staffId, socket))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var staffRepository = scope.ServiceProvider.GetRequiredService<IStaffRepository>();
                    var notificationLogsRepository = scope.ServiceProvider.GetRequiredService<INotificationLogsRepository>();
                    var staff = await staffRepository.GetAsync(staffId);
                    var messageBefore = await notificationLogsRepository.GetAllNotificationLogsAsync(staff.CompanyId);
                    var messageAll = await notificationLogsRepository.GetCompanyNotificationLogsAsync(staff.CompanyId);
                    var messageDepartment = await notificationLogsRepository.GetDepartmentNotificationLogsAsync(staff.CompanyId, staff.DepartmentId);
                    var messagePersonal = await notificationLogsRepository.GetStaffNotificationLogsAsync(staff.CompanyId, staffId);
                    // 將三个列表组合成一个
                    var combinedMessages = messageAll.Concat(messageDepartment).Concat(messagePersonal).Concat(messageBefore).OrderBy(x => x.IsUnRead).ToList();
                    var combinedMessageJson = JsonConvert.SerializeObject(combinedMessages);
                    var buffer = Encoding.UTF8.GetBytes(combinedMessageJson);
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async Task RemoveSocketAsync(int staffId)
        {
            if (_webSockets.TryRemove(staffId, out var socket) && socket != null)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by WebSocketHandler", CancellationToken.None);
            }
        }


        public async Task SendMessageToUserAsync(int staffId, string message)
        {
            if (_webSockets.TryGetValue(staffId, out var userSocket) && userSocket.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await userSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendMessageToSpecificUsersAsync(Dictionary<int, string> staffIdMessageMap)
        {
            foreach (var item in staffIdMessageMap)
            {
                int staffId = item.Key;
                string message = item.Value;

                if (_webSockets.TryGetValue(staffId, out var userSocket) && userSocket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await userSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }



        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in _webSockets)
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        public virtual async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await SendMessageToAllAsync(message);
        }
    }
}
