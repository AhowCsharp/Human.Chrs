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
using Human.Chrs.Domain.Helper;
using System.Collections;

using Microsoft.EntityFrameworkCore;

namespace LineTag.Infrastructure.Repositories
{
    public class SalarySettingRepository : BaseRepository<SalarySetting, SalarySettingDTO, int>, ISalarySettingRepository
    {
        public SalarySettingRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<SalarySettingDTO> GetSalarySettingAsync(int staffId, int companyId)
        {
            var data = await _context.SalarySetting.FirstOrDefaultAsync(x => x.StaffId == staffId && x.CompanyId == companyId);

            return _mapper.Map<SalarySettingDTO>(data);
        }

        public async Task<IEnumerable<SalarySettingDTO>> GetAllSalarySettingAsync(int companyId)
        {
            var data = await _context.SalarySetting.Where(x => x.CompanyId == companyId).ToListAsync();

            return data.Select(_mapper.Map<SalarySettingDTO>);
        }
    }
}