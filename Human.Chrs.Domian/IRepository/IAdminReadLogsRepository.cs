using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.SeedWork;
using System.Security.Principal;

namespace Human.Chrs.Domain.IRepository
{
    public interface IAdminReadLogsRepository : IRepository<AdminReadLogsDTO, int>
    {
        Task<bool> GetReadStatus(int adminId, int adminNotificationId);
    }
}