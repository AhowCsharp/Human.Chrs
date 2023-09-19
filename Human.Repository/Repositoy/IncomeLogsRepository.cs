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
    public class IncomeLogsRepository : BaseRepository<IncomeLogs, IncomeLogsDTO, int>, IIncomeLogsRepository
    {
        public IncomeLogsRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<IEnumerable<IncomeLogsDTO>> GetIncomeLogsAsync(int staffId, int companyId)
        {
            var data = await _context.IncomeLogs
                .Where(x => x.StaffId == staffId && x.CompanyId == companyId)
                .OrderByDescending(x => x.IssueDate)
                .Take(12)
                .ToListAsync();

            return data.Select(_mapper.Map<IncomeLogsDTO>);
        }
    }
}