using Human.Chrs.Domain.Websocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.DTO;
using System.Net.WebSockets;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace Human.Chrs.Domain.Services
{
    public class SqlNotificationService : IDisposable
    {
        private readonly SqlTableDependency<NotificationLogsEntity> _tableDependency;
        private readonly WebSocketHandler _webSocketHandler;

        public SqlNotificationService(string connectionString, WebSocketHandler webSocketHandler)
        {
            _webSocketHandler = webSocketHandler;

            _tableDependency = new SqlTableDependency<NotificationLogsEntity>(connectionString);
            _tableDependency.OnChanged += TableDependency_OnChanged;
            _tableDependency.Start();
        }

        private async void TableDependency_OnChanged(object sender, RecordChangedEventArgs<NotificationLogsEntity> e)
        {
            if (e.ChangeType == ChangeType.Insert)
            {
                var changedEntity = e.Entity;
                var jsonMessage = JsonConvert.SerializeObject(changedEntity);
                await _webSocketHandler.SendMessageToAllAsync(jsonMessage);
            }
        }

        public void Dispose()
        {
            _tableDependency.Stop();
        }
    }
}
