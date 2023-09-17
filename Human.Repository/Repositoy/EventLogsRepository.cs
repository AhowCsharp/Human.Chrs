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
    public class EventLogsRepository : BaseRepository<EventLogs, EventLogsDTO, int>, IEventLogsRepository
    {
        public EventLogsRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<IEnumerable<EventLogsDTO>> GetAllEventLogsAsync(int staffId, int companyId)
        {
            var data = await _context.EventLogs.Where(x => x.StaffId == staffId && x.CompanyId == companyId).ToListAsync();

            return data.Select(_mapper.Map<EventLogsDTO>);
        }

    }
}