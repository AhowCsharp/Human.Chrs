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
    public class ResetPasswordLogsRepository : BaseRepository<ResetPasswordLogs, ResetPasswordLogsDTO, int>, IResetPasswordLogsRepository
    {
        public ResetPasswordLogsRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<bool> GetResetLog(int staffId, int companyId)
        {
            DateTime fiveMinutesAgo = DateTimeHelper.TaipeiNow.AddMinutes(-5);
            var data = await _context.ResetPasswordLogs.AnyAsync(x => x.StaffId == staffId && x.CompanyId == companyId && x.CreateDate >= fiveMinutesAgo);

            return data;
        }
    }
}