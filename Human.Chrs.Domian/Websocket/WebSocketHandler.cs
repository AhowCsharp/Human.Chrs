using Human.Chrs.Domain.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Net.Mail;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Human.Chrs.Domain.Websocket
{
    public class WebSocketHandler
    {
        private static ConcurrentDictionary<int, WebSocket> _webSockets = new ConcurrentDictionary<int, WebSocket>();
        private static ConcurrentDictionary<string, WebSocket> _adminWebSockets = new ConcurrentDictionary<string, WebSocket>();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WebSocketHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<bool> VerifyAdminToken(string adminToken, int adminId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var adminRepository = scope.ServiceProvider.GetRequiredService<IAdminRepository>();
                return await adminRepository.VerifyWebSocketAdminTokenAsync(adminToken, adminId);
            }
        }

        public async Task AddAdminSocket(string adminToken, int adminId, WebSocket socket)
        {
            if (_adminWebSockets.TryAdd(adminToken, socket))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        var adminRepository = scope.ServiceProvider.GetRequiredService<IAdminRepository>();
                        var adminReadLogs = scope.ServiceProvider.GetRequiredService<IAdminReadLogsRepository>();
                        var adminNotificationRepository = scope.ServiceProvider.GetRequiredService<IAdminNotificationLogsRepository>();
                        var admin = await adminRepository.GetAsync(adminId);
                        var messageAll = (await adminNotificationRepository.GetAdminCompanyNotificationLogsAsync(admin.CompanyId)).ToList();
                        foreach (var mes in messageAll)
                        {
                            if (await adminReadLogs.GetReadStatus(admin.id, mes.id))
                            {
                                mes.IsUnRead = false;
                            }
                            else
                            {
                                mes.IsUnRead = true;
                            }
                        }

                        var messageAllJson = JsonConvert.SerializeObject(messageAll);
                        var buffer = Encoding.UTF8.GetBytes(messageAllJson);
                        await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        // Handle or log the exception as needed
                        Console.WriteLine($"Error in AddAdminSocket: {ex.Message}");
                        // You can also consider closing the socket if an error occurs.
                        await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Error occurred", CancellationToken.None);
                    }
                }
            }
        }

        public async Task AddStaffSocket(int staffId, WebSocket socket)
        {
            if (_webSockets.TryAdd(staffId, socket))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var staffRepository = scope.ServiceProvider.GetRequiredService<IStaffRepository>();
                    var notificationLogsRepository = scope.ServiceProvider.GetRequiredService<INotificationLogsRepository>();
                    var readLogsRepository = scope.ServiceProvider.GetRequiredService<IReadLogsRepository>();

                    var staff = await staffRepository.GetAsync(staffId);
                    var messageAll = (await notificationLogsRepository.GetCompanyNotificationLogsAsync(staff.CompanyId)).ToList();
                    var messageDepartment = (await notificationLogsRepository.GetDepartmentNotificationLogsAsync(staff.CompanyId, staff.DepartmentId)).ToList();
                    var messagePersonal = await notificationLogsRepository.GetStaffNotificationLogsAsync(staff.CompanyId, staffId);

                    foreach (var message in messageAll)
                    {
                        var isRead = await readLogsRepository.GetReadStatus(staffId, message.id);
                        if (isRead)
                        {
                            message.IsUnRead = false;
                        }
                        else
                        {
                            message.IsUnRead = true;
                        }
                    }
                    foreach (var message in messageDepartment)
                    {
                        var isRead = await readLogsRepository.GetReadStatus(staffId, message.id);
                        if (isRead)
                        {
                            message.IsUnRead = false;
                        }
                        else
                        {
                            message.IsUnRead = true;
                        }
                    }

                    var combinedMessages = messageAll.Concat(messageDepartment).Concat(messagePersonal).OrderByDescending(x => x.CreateDate).ToList();
                    var combinedMessageJson = JsonConvert.SerializeObject(combinedMessages);
                    var buffer = Encoding.UTF8.GetBytes(combinedMessageJson);
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async Task RemoveStaffSocketAsync(int staffId)
        {
            if (_webSockets.TryRemove(staffId, out var socket) && socket != null)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by WebSocketHandler", CancellationToken.None);
            }
        }

        public async Task RemoveAdminSocketAsync(string adminToken)
        {
            if (_adminWebSockets.TryRemove(adminToken, out var socket) && socket != null)
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

        public async Task SendMessageToSpecificStaffssAsync(Dictionary<int, string> staffIdMessageMap)
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

        public async Task SendMessageToSpecificAdminsAsync(Dictionary<string, string> adminMessageMap)
        {
            foreach (var item in adminMessageMap)
            {
                string adminToken = item.Key;
                string message = item.Value;

                if (_adminWebSockets.TryGetValue(adminToken, out var adminSocket) && adminSocket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await adminSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
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