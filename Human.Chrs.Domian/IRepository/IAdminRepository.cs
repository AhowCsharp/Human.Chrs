using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.CommonModels;

namespace Human.Chrs.Domain.IRepository
{
    public interface IAdminRepository : IRepository<AdminDTO, int>
    {
        Task<AdminDTO> GetAvailableAdminAsync(int id, int companyId);

        Task<AdminDTO> VerifyLoginAdminAsync(string account, string password);

        Task<bool> VerifyAdminTokenAsync(CurrentUser admin);

        Task<IEnumerable<AdminDTO>> GetAllAdminsAsync(int companyId);

        Task<bool> VerifyWebSocketAdminTokenAsync(string token, int adminId);
    }
}