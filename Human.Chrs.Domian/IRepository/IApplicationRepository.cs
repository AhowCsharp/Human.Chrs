using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain.IRepository.Base;

namespace Human.Chrs.Domain.IRepository
{
    public interface IApplicationRepository : IRepository<ApplicationDTO, int>
    {
        Task<ApplicationDTO> GetAsync(string token);
    }
}