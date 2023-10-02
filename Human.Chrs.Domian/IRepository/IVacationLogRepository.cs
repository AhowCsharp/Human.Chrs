using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.SeedWork;
using System.Security.Principal;

namespace Human.Chrs.Domain.IRepository
{
    public interface IVacationLogRepository : IRepository<VacationLogDTO, int>
    {
        Task<IEnumerable<VacationLogDTO>> GetTop5VacationLogsAsync(int staffId, int companyId);

        Task<bool> VerifyVacationLogsAsync(int staffId, int companyId, DateTime start, DateTime end);

        Task<IEnumerable<VacationLogDTO>> GetPeriodVacationLogsAsync(int staffId, int companyId, DateTime start, DateTime end);

        Task<IEnumerable<VacationLogDTO>> GetPeriodVacationLogsAsync(int companyId, DateTime start, DateTime end);
    }
}