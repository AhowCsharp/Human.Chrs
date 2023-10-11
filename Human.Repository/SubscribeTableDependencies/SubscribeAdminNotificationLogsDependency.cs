using Human.Chrs.Domain.DTO;
using TableDependency.SqlClient;
using Human.Repository.EF;
using Human.Chrs.Domain.IRepository;
using Newtonsoft.Json;
using Human.Chrs.Domain.Websocket;
using Microsoft.Extensions.DependencyInjection;
using TableDependency.SqlClient.Base.Messages;

namespace Human.Repository.SubscribeTableDependencies
{
    public class SubscribeAdminNotificationLogsDependency : ISubscribeTableDependency
    {
        private SqlTableDependency<AdminNotificationLogs> tableDependency;
        private string _connectionString;
        private readonly WebSocketHandler _webSocketHandler;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SubscribeAdminNotificationLogsDependency(string connectionString, IServiceScopeFactory serviceScopeFactory, WebSocketHandler webSocketHandler)
        {
            _connectionString = connectionString;
            _serviceScopeFactory = serviceScopeFactory;
            _webSocketHandler = webSocketHandler;
        }

        public void SubscribeTableDependency()
        {
            tableDependency = new SqlTableDependency<AdminNotificationLogs>(_connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<AdminNotificationLogs> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var adminRepository = scope.ServiceProvider.GetRequiredService<IAdminRepository>();
                    var adminNotificationLogsRepository = scope.ServiceProvider.GetRequiredService<IAdminNotificationLogsRepository>();
                    var adminReadLogs = scope.ServiceProvider.GetRequiredService<IAdminReadLogsRepository>();

                    var changedEntity = e.Entity;
                    var allAdmins = await adminRepository.GetAllAdminsAsync(changedEntity.CompanyId);

                    var allMessage = (await adminNotificationLogsRepository.GetAdminCompanyNotificationLogsAsync(changedEntity.CompanyId)).ToList().OrderByDescending(x => x.CreateDate);
                    var messagesForAdmins = new Dictionary<string, string>();
                    foreach (var admin in allAdmins)
                    {
                        foreach (var mes in allMessage)
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
                        var MessageJson = JsonConvert.SerializeObject(allMessage);
                        messagesForAdmins.Add(admin.AdminToken, MessageJson);
                    }

                    await _webSocketHandler.SendMessageToSpecificAdminsAsync(messagesForAdmins);
                }
            }
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(NotificationLogs)} SqlTableDependency error: {e.Error.Message}");
        }
    }
}