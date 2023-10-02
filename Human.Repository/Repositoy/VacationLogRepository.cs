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
    public class VacationLogRepository : BaseRepository<VacationLog, VacationLogDTO, int>, IVacationLogRepository
    {
        public VacationLogRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<IEnumerable<VacationLogDTO>> GetTop5VacationLogsAsync(int staffId, int companyId)
        {
            var data = await _context.VacationLog
                .Where(x => x.StaffId == staffId && x.CompanyId == companyId)
                .OrderByDescending(x => x.ApplyDate) // 按照時間降序排序
                .Take(3) // 選擇前 count 筆資料
                .ToListAsync();

            return data.Select(_mapper.Map<VacationLogDTO>);
        }

        public async Task<IEnumerable<VacationLogDTO>> GetPeriodVacationLogsAsync(int staffId, int companyId, DateTime start, DateTime end)
        {
            var data = await _context.VacationLog
                .Where(x => x.StaffId == staffId && x.CompanyId == companyId && x.ActualStartDate >= start && x.ActualStartDate <= end)
                .OrderByDescending(x => x.ApplyDate) // 按照時間降序排序
                .ToListAsync();

            return data.Select(_mapper.Map<VacationLogDTO>);
        }

        public async Task<IEnumerable<VacationLogDTO>> GetPeriodVacationLogsAsync(int companyId, DateTime start, DateTime end)
        {
            var data = await _context.VacationLog
                .Where(x => x.CompanyId == companyId && x.ActualStartDate >= start && x.ActualStartDate <= end)
                .OrderByDescending(x => x.StaffId)
                .ToListAsync();

            return data.Select(_mapper.Map<VacationLogDTO>);
        }

        public async Task<bool> VerifyVacationLogsAsync(int staffId, int companyId, DateTime start, DateTime end)
        {
            var data = await _context.VacationLog
                .AnyAsync(x => x.StaffId == staffId && x.CompanyId == companyId && x.ActualStartDate <= end
                      && x.ActualEndDate >= start);

            return data;
        }
    }
}