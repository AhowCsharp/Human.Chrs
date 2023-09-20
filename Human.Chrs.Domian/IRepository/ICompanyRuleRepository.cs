using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;

namespace Human.Chrs.Domain.IRepository
{
    public interface ICompanyRuleRepository : IRepository<CompanyRuleDTO, int>
    {
        Task<CompanyRuleDTO> GetCompanyRuleAsync(int companyId, int DepartmentId);

        Task<IEnumerable<CompanyRuleDTO>> GetCompanyRulesAsync(int companyId);
    }
}