using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.SeedWork;
using System.Security.Principal;

namespace Human.Chrs.Domain.IRepository
{
    public interface IReadLogsRepository : IRepository<ReadLogsDTO, int>
    {
        Task<bool> GetReadStatus(int staffId, int notificationId);
    }
}