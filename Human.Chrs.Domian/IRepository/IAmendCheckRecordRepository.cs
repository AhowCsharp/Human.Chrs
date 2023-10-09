using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.SeedWork;
using System.Security.Principal;

namespace Human.Chrs.Domain.IRepository
{
    public interface IAmendCheckRecordRepository : IRepository<AmendCheckRecordDTO, int>
    {
        Task<IEnumerable<AmendCheckRecordDTO>> GetAllAmendCheckRecordAsync(int companyId, DateTime start, DateTime end);

        Task<IEnumerable<AmendCheckRecordDTO>> GetTop6AmendCheckRecordAsync(int staffId, int companyId);
    }
}