using Human.Chrs.Domain.DTO;

namespace Human.Chrs.Domain.IRepository
{
    public interface IApplicationRepository
    {
        Task<ApplicationDTO> GetAsync(string token);
    }
}