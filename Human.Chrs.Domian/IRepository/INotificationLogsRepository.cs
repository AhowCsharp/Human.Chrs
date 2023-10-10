using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.SeedWork;
using System.Security.Principal;

namespace Human.Chrs.Domain.IRepository
{
    public interface INotificationLogsRepository : IRepository<NotificationLogsDTO, int>
    {
        Task<IEnumerable<NotificationLogsDTO>> GetCompanyNotificationLogsAsync(int companyId);

        Task<IEnumerable<NotificationLogsDTO>> GetAllNotificationLogsAsync(int companyId);

        Task<IEnumerable<NotificationLogsDTO>> GetDepartmentNotificationLogsAsync(int companyId, int departmentId);

        Task<IEnumerable<NotificationLogsDTO>> GetStaffNotificationLogsAsync(int companyId, int staffId);
    }
}