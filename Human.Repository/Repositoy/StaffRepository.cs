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
    public class StaffRepository : BaseRepository<Staff, StaffDTO, int>, IStaffRepository
    {
        public StaffRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<StaffDTO> VerifyLoginStaffAsync(string account, string password)
        {
            var data = await _context.Staff.SingleOrDefaultAsync(x => x.StaffAccount == account && x.StaffPassWord == password && x.Status == 1);

            return _mapper.Map<StaffDTO>(data);
        }

        public async Task<bool> VerifyExistStaffAsync(int staffId, int companyId)
        {
            var data = await _context.Staff.AnyAsync(x => x.Id == staffId && x.CompanyId == companyId && x.Status == 1);

            return data;
        }

        public async Task<StaffDTO> GetUsingStaffAsync(int staffId, int companyId)
        {
            var data = await _context.Staff.SingleOrDefaultAsync(x => x.Id == staffId && x.CompanyId == companyId && x.Status == 1);

            return _mapper.Map<StaffDTO>(data);
        }

        public async Task<IEnumerable<StaffDTO>> GetAllStaffAsync(int staffId, int companyId)
        {
            var data = await _context.Staff.Where(x => x.Id == staffId && x.CompanyId == companyId).ToListAsync();

            return data.Select(_mapper.Map<StaffDTO>);
        }
    }
}