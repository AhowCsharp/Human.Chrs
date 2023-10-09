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

namespace LineTag.Infrastructure.Repositories
{
    public class CheckRecordsRepository : BaseRepository<CheckRecords, CheckRecordsDTO, int>, ICheckRecordsRepository
    {
        public CheckRecordsRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<CheckRecordsDTO> GetCheckRecordAsync(int companyId, int staffId)
        {
            DateTime today = DateTimeHelper.TaipeiNow.Date; // 取得台北今天的日期，時間設為 00:00:00
            DateTime tomorrow = DateTimeHelper.TaipeiNow.Date.AddDays(1);
            var data = await _context.CheckRecords.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.StaffId == staffId &&
                        x.CheckInTime >= today && x.CheckInTime < tomorrow);
            return _mapper.Map<CheckRecordsDTO>(data);
        }

        public async Task<IEnumerable<CheckRecordsDTO>> GetCheckRecordListAsync(int staffId, int companyId, DateTime start, DateTime end)
        {
            var data = await _context.CheckRecords.Where(x => x.CompanyId == companyId && x.StaffId == staffId &&
                        x.CheckInTime >= start && x.CheckInTime <= end).OrderByDescending(x => x.CheckInTime).ToListAsync();

            return data.Select(_mapper.Map<CheckRecordsDTO>);
        }

        public async Task<CheckRecordsDTO> GetCheckRecordPeriodAsync(int staffId, int companyId, DateTime start, DateTime end)
        {
            var data = await _context.CheckRecords.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.StaffId == staffId &&
                        x.CheckInTime >= start && x.CheckInTime <= end);

            return _mapper.Map<CheckRecordsDTO>(data);
        }
    }
}