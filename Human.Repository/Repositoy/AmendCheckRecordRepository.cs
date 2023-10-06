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
using System.Drawing;

namespace LineTag.Infrastructure.Repositories
{
    public class AmendCheckRecordRepository : BaseRepository<AmendCheckRecord, AmendCheckRecordDTO, int>, IAmendCheckRecordRepository
    {
        public AmendCheckRecordRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<IEnumerable<AmendCheckRecordDTO>> GetAllAmendCheckRecordAsync(int companyId)
        {
            var data = await _context.AmendCheckRecord
                .Where(x => x.CompanyId == companyId)
                .OrderByDescending(x => x.CheckDate)
                .ToListAsync();

            return data.Select(_mapper.Map<AmendCheckRecordDTO>);
        }

        public async Task<IEnumerable<AmendCheckRecordDTO>> GetTop6AmendCheckRecordAsync(int staffId, int companyId)
        {
            var data = await _context.AmendCheckRecord
                .Where(x => x.CompanyId == companyId && x.StaffId == staffId)
                .OrderByDescending(x => x.CheckDate)
                .Take(6)
                .ToListAsync();

            return data.Select(_mapper.Map<AmendCheckRecordDTO>);
        }
    }
}