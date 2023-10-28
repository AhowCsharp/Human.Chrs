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
    public class ReadLogsRepository : BaseRepository<ReadLogs, ReadLogsDTO, int>, IReadLogsRepository
    {
        public ReadLogsRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<bool> GetReadStatus(int staffId, int notificationId)
        {
            var data = await _context.ReadLogs.AnyAsync(x => x.StaffId == staffId && x.NotificationLogId == notificationId);

            return data;
        }
    }
}