using Human.Chrs.Domain.DTO;
using TableDependency.SqlClient;
using Human.Repository.EF;
using Human.Chrs.Domain.IRepository;
using Newtonsoft.Json;
using Human.Chrs.Domain.Websocket;
using Microsoft.Extensions.DependencyInjection;

namespace Human.Repository.SubscribeTableDependencies
{
    public class SubscribeNotificationLogsDependency : ISubscribeTableDependency
    {
        private SqlTableDependency<NotificationLogs> tableDependency;
        private string _connectionString;
        private readonly WebSocketHandler _webSocketHandler;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SubscribeNotificationLogsDependency(string connectionString, IServiceScopeFactory serviceScopeFactory, WebSocketHandler webSocketHandler)
        {
            _connectionString = connectionString;
            _serviceScopeFactory = serviceScopeFactory;
            _webSocketHandler = webSocketHandler;
        }

        public void SubscribeTableDependency()
        {
            tableDependency = new SqlTableDependency<NotificationLogs>(_connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<NotificationLogs> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var staffRepository = scope.ServiceProvider.GetRequiredService<IStaffRepository>();
                    var notificationLogsRepository = scope.ServiceProvider.GetRequiredService<INotificationLogsRepository>();
                    var readLogsRepository = scope.ServiceProvider.GetRequiredService<IReadLogsRepository>();

                    var changedEntity = e.Entity;
                    var staffIds = (await staffRepository.GetAllStaffAsync(changedEntity.CompanyId)).Select(x => x.id);
                    var messagesForUsers = new Dictionary<int, string>();
                    foreach (var staffId in staffIds)
                    {
                        var staff = await staffRepository.GetAsync(staffId);
                        var messageAll = (await notificationLogsRepository.GetCompanyNotificationLogsAsync(changedEntity.CompanyId)).ToList();
                        var messageDepartment = (await notificationLogsRepository.GetDepartmentNotificationLogsAsync(changedEntity.CompanyId, staff.DepartmentId)).ToList();
                        var messagePersonal = await notificationLogsRepository.GetStaffNotificationLogsAsync(changedEntity.CompanyId, staffId);

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

                        // 將三个列表组合成一个
                        var combinedMessages = messageAll.Concat(messageDepartment).Concat(messagePersonal).ToList();

                        // 转换为 JSON
                        var combinedMessageJson = JsonConvert.SerializeObject(combinedMessages);

                        messagesForUsers[staffId] = combinedMessageJson;
                    }
                    await _webSocketHandler.SendMessageToSpecificStaffssAsync(messagesForUsers);
                }
            }
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(NotificationLogs)} SqlTableDependency error: {e.Error.Message}");
        }
    }
}