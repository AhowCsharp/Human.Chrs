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

        public async Task<bool> CheckInAsync(CheckRecordsDTO dto)
        {
            var today = DateTimeHelper.TaipeiNow.Date;
            var tomorrow = today.AddDays(1);
            var data = await _context.CheckRecords.AnyAsync(x => x.CompanyId == dto.CompanyId && x.StaffId == dto.StaffId &&
            x.CheckInTime >= today && x.CheckInTime < tomorrow);
            if (data)
            {
                return false;
            }
            else
            {
                await InsertAsync(dto);
                return true;
            }
        }
    }
}