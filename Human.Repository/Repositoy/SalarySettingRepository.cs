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

namespace Human.Chrs.Infrastructure.Repositories
{
    public class SalarySettingRepository : BaseRepository<SalarySetting, SalarySettingDTO, int>, ISalarySettingRepository
    {
        public SalarySettingRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<SalarySettingDTO> GetSalarySettingAsync(int staffId, int companyId)
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(x => x.Id == staffId);
            if (staff.EmploymentTypeId != 1 && staff.EmploymentTypeId != 4)
            {
                return _mapper.Map<SalarySettingDTO>(null);
            }
            var data = await _context.SalarySetting.FirstOrDefaultAsync(x => x.StaffId == staffId && x.CompanyId == companyId);
            return _mapper.Map<SalarySettingDTO>(data);
        }

        public async Task<IEnumerable<SalarySettingDTO>> GetAllSalarySettingAsync(int companyId)
        {
            // 首先, 获取所有符合条件的员工ID
            var includedStaffIds = await _context.Staff
                .Where(x => x.CompanyId == companyId && (x.EmploymentTypeId == 1 || x.EmploymentTypeId == 4))
                .Select(x => x.Id)
                .ToListAsync();

            // 接下来, 获取那些员工ID与上述集合匹配的薪资设置
            var data = await _context.SalarySetting
                .Where(x => x.CompanyId == companyId && includedStaffIds.Contains(x.StaffId)) // 假设SalarySetting有一个StaffId字段与Staff的Id字段对应
                .ToListAsync();

            // 最后, 转换实体到DTOs
            var result = data.Select(item => _mapper.Map<SalarySettingDTO>(item));

            return result;
        }

    }
}