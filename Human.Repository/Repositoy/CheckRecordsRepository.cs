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

namespace Human.Chrs.Infrastructure.Repositories
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

        public async Task<bool> GetStaffCheckInStatus(int staffId)
        {

            var today = DateTimeHelper.TaipeiNow.Date;
            var tomorrow = today.AddDays(1);

            // 查询是否有任何记录的 CheckInTime 在这两个时间点之间
            var hasCheckedInToday = await _context.CheckRecords.AnyAsync(x =>
                x.StaffId == staffId &&
                x.CheckInTime >= today &&
                x.CheckInTime < tomorrow && x.CheckInTime != null);

            return hasCheckedInToday;
        }

        public async Task<bool> GetStaffCheckOutStatus(int staffId)
        {
            // 获取昨天日期的开始和结束时间
            var today = DateTimeHelper.TaipeiNow.Date;
            var yesterday = today.AddDays(-1);
            var startOfToday = today;

            // 查询是否有任何记录的 CheckInTime 在这两个时间点之间
            var hasCheckedOutYesterday = await _context.CheckRecords.AnyAsync(x =>
                x.StaffId == staffId &&
                x.CheckInTime >= yesterday &&
                x.CheckInTime < startOfToday&&
                x.CheckOutTime != null);

            return hasCheckedOutYesterday;
        }
    }
}