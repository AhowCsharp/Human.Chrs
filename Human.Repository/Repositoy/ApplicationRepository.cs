using AutoMapper;
using Human.Chrs.Domain.DTO;
using Human.Chrs.Domain;
using Microsoft.EntityFrameworkCore;
using Human.Chrs.Domain.IRepository;
using System.Threading.Tasks;
using Human.Repository.EF;

namespace Human.Repository.Repository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IMapper _mapper;
        private readonly HumanChrsContext _context;

        public ApplicationRepository(IMapper mapper, HumanChrsContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ApplicationDTO> GetAsync(string token)
        {
            var data = await _context.Application.SingleOrDefaultAsync(x => x.Token == token && x.Status);

            return _mapper.Map<ApplicationDTO>(data);
        }
    }
}