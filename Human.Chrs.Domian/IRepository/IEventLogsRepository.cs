using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;
using Human.Chrs.Domain.SeedWork;
using System.Security.Principal;

namespace Human.Chrs.Domain.IRepository
{
    public interface IEventLogsRepository : IRepository<EventLogsDTO, int>
    {
        Task<IEnumerable<EventLogsDTO>> GetAllEventLogsAsync(int staffId, int companyId);

        Task<bool> RemoveEventLogsAsync(IEnumerable<EventLogsDTO> logsToDelete, int companyId);

        Task<IEnumerable<EventLogsDTO>> GetCompanyPartimeEventLogsAsync(int companyId);
    }
}