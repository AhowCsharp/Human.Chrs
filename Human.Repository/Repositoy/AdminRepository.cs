using AutoMapper;
using EFCore.BulkExtensions;
using Human.Chrs.Domain.IRepository;
using Newtonsoft.Json;
using Human.Repository.Repository.Base;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Human.Repository.EF;
using Human.Chrs.Domain.DTO;

namespace LineTag.Infrastructure.Repositories
{
    public class AdminRepository : BaseRepository<Admin, AdminDTO, int>, IAdminRepository
    {
        public AdminRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<AdminDTO> GetAvailableAdminAsync(int companyId, string account)
        {
            var data = await _context.Admin.SingleOrDefaultAsync(x => x.Account == account && x.CompanyId == companyId);

            return _mapper.Map<AdminDTO>(data);
        }

        public async Task<AdminDTO> VerifyLoginAdminAsync(string account, string password)
        {
            var data = await _context.Admin.SingleOrDefaultAsync(x => x.Account == account && x.Password == password);

            return _mapper.Map<AdminDTO>(data);
        }
    }
}