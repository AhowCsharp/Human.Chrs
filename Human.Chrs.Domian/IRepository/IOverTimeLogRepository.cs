using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;

namespace Human.Chrs.Domain.IRepository
{
    public interface IOverTimeLogRepository : IRepository<OverTimeLogDTO, int>
    {
        Task<OverTimeLogDTO> GetOverTimeLogAsync(int staffId, int companyId);
    }
}