using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;

namespace Human.Chrs.Domain.IRepository
{
    public interface IAdminRepository : IRepository<AdminDTO, int>
    {
        Task<AdminDTO> GetAvailableAdminAsync(int companyId, string userId);
    }
}