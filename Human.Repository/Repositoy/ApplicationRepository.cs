using AutoMapper;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain;
using Microsoft.EntityFrameworkCore;
using Human.Chrs.Domain.IRepository;
using System.Threading.Tasks;
using Human.Repository.EF;
using Human.Repository.Repository.Base;

namespace Human.Repository.Repository
{
    public class ApplicationRepository : BaseRepository<Application, ApplicationDTO, int>, IApplicationRepository
    {
        public ApplicationRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<ApplicationDTO> GetAsync(string token)
        {
            var data = await _context.Application.SingleOrDefaultAsync(x => x.Token == token && x.Status);

            return _mapper.Map<ApplicationDTO>(data);
        }
    }
}