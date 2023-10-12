using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.SeedWork;
using System.Security.Principal;

namespace Human.Chrs.Domain.IRepository
{
    public interface IResetPasswordLogsRepository : IRepository<ResetPasswordLogsDTO, int>
    {
        Task<bool> GetResetLog(int staffId, int companyId);
    }
}