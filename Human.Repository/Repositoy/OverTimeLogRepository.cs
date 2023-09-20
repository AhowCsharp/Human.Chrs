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

namespace LineTag.Infrastructure.Repositories
{
    public class OverTimeLogRepository : BaseRepository<OverTimeLog, OverTimeLogDTO, int>, IOverTimeLogRepository
    {
        public OverTimeLogRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }
        public async Task<OverTimeLogDTO> GetOverTimeLogAsync(int staffId,int companyId)
        {
            DateTime today = DateTimeHelper.TaipeiNow.Date; // 取得台北今天的日期，時間設為 00:00:00
            DateTime tomorrow = DateTimeHelper.TaipeiNow.Date.AddDays(1);
            var data = await _context.OverTimeLog.FirstOrDefaultAsync(x => x.StaffId == staffId && x.CompanyId == companyId
            && x.OvertimeDate == today );
            return _mapper.Map<OverTimeLogDTO>(data);
        }

        public async Task<IEnumerable<OverTimeLogDTO>> GetOverTimeLogOfPeriodAsync(int staffId, int companyId,DateTime start,DateTime end)
        {
            var data = await _context.OverTimeLog.Where(x => x.StaffId == staffId && x.CompanyId == companyId
            && x.OvertimeDate >= start && x.OvertimeDate <= end).ToListAsync();
            return data.Select(_mapper.Map<OverTimeLogDTO>);
        }
    }
}