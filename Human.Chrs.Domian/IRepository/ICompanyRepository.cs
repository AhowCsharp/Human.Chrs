using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;

namespace Human.Chrs.Domain.IRepository
{
    public interface ICompanyRepository : IRepository<CompanyDTO, int>
    {
        Task<bool> IsAvailableCompanyAsync(int companyId);

        Task<IEnumerable<CompanyDTO>> AllCompanyAsync();
    }
}