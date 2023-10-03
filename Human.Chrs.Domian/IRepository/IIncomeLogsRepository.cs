using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.SeedWork;
using System.Security.Principal;

namespace Human.Chrs.Domain.IRepository
{
    public interface IIncomeLogsRepository : IRepository<IncomeLogsDTO, int>
    {
        Task<IEnumerable<IncomeLogsDTO>> GetIncomeLogsAsync(int staffId, int companyId);

        Task<bool> IsRepeatPayAsync(int staffId, int companyId, int salaryOfMonth);
    }
}