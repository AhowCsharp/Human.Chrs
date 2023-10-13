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
using Human.Chrs.Domain.CommonModels;

namespace LineTag.Infrastructure.Repositories
{
    public class AdminRepository : BaseRepository<Admin, AdminDTO, int>, IAdminRepository
    {
        public AdminRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<AdminDTO> GetAvailableAdminAsync(int id, int companyId)
        {
            var data = await _context.Admin.SingleOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId && x.Status != null && x.Status.Value);

            return _mapper.Map<AdminDTO>(data);
        }

        public async Task<IEnumerable<AdminDTO>> GetAllAdminsAsync(int companyId)
        {
            var data = await _context.Admin.Where(x => x.CompanyId == companyId).ToListAsync();

            return data.Select(_mapper.Map<AdminDTO>);
        }

        public async Task<int> GetAllAdminsCountAsync(int companyId)
        {
            var data = await _context.Admin.Where(x => x.CompanyId == companyId).CountAsync();

            return data;
        }

        public async Task<AdminDTO> VerifyLoginAdminAsync(string account, string password)
        {
            var data = await _context.Admin.SingleOrDefaultAsync(x => x.Account == account && x.Password == password && x.Status != null && x.Status.Value);

            return _mapper.Map<AdminDTO>(data);
        }

        public async Task<bool> VerifyAdminTokenAsync(CurrentUser admin)
        {
            var data = await _context.Admin.AnyAsync(x => x.Id == admin.Id && x.CompanyId == admin.CompanyId && x.AdminToken == admin.AdminToken);

            return data;
        }

        public async Task<bool> VerifyWebSocketAdminTokenAsync(string token, int adminId)
        {
            var data = await _context.Admin.AnyAsync(x => x.Id == adminId && x.AdminToken == token);

            return data;
        }

        public async Task<bool> VerifyAdminAccountAsync(string account)
        {
            var data = await _context.Admin.AnyAsync(x => x.Account == account);

            return data;
        }

        public async Task<bool> VerifyAdminAccountAsync(string account, int adminId)
        {
            if (adminId == 0)
            {
                var data = await _context.Admin.AnyAsync(x => x.Account == account);
                return data;
            }
            else
            {
                var data = await _context.Admin.AnyAsync(x => x.Account == account && x.Id != adminId);
                return data;
            }
        }
    }
}