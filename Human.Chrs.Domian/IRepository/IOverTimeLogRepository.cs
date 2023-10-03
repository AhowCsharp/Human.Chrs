using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;

namespace Human.Chrs.Domain.IRepository
{
    public interface IOverTimeLogRepository : IRepository<OverTimeLogDTO, int>
    {
        Task<OverTimeLogDTO> GetOverTimeLogAsync(int staffId, int companyId);

        Task<IEnumerable<OverTimeLogDTO>> GetOverTimeLogOfPeriodAsync(int staffId, int companyId, DateTime start, DateTime end);

        Task<IEnumerable<OverTimeLogDTO>> GetOverTimeLogOfPeriodAfterValidateAsync(int staffId, int companyId, DateTime start, DateTime end);

        Task<IEnumerable<OverTimeLogDTO>> GetOverTimeLogOfPeriodAsync(int companyId, DateTime start, DateTime end);
    }
}