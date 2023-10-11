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
    public class AdminNotificationLogsRepository : BaseRepository<AdminNotificationLogs, AdminNotificationLogsDTO, int>, IAdminNotificationLogsRepository
    {
        public AdminNotificationLogsRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<IEnumerable<AdminNotificationLogsDTO>> GetAdminCompanyNotificationLogsAsync(int companyId)
        {
            var data = await _context.AdminNotificationLogs.Where(x => x.CompanyId == companyId && x.IsUnRead == true).ToListAsync();

            return data.Select(_mapper.Map<AdminNotificationLogsDTO>);
        }
    }
}