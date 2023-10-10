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
    public class NotificationLogsRepository : BaseRepository<NotificationLogs, NotificationLogsDTO, int>, INotificationLogsRepository
    {
        public NotificationLogsRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<IEnumerable<NotificationLogsDTO>> GetCompanyNotificationLogsAsync(int companyId)
        {
            var data = await _context.NotificationLogs.Where(x => x.CompanyId == companyId && x.StaffId == 0 
            && x.DepartmentId == 0 && x.IsUnRead == true).ToListAsync();

            return data.Select(_mapper.Map<NotificationLogsDTO>);
        }

        public async Task<IEnumerable<NotificationLogsDTO>> GetAllNotificationLogsAsync(int companyId)
        {
            DateTime sixHoursAgo = DateTimeHelper.TaipeiNow.AddHours(-48);
            var data = await _context.NotificationLogs.Where(x => x.CompanyId == companyId 
            && x.CreateDate >= sixHoursAgo && x.IsUnRead == false).ToListAsync();

            return data.Select(_mapper.Map<NotificationLogsDTO>);
        }

        public async Task<IEnumerable<NotificationLogsDTO>> GetDepartmentNotificationLogsAsync(int companyId,int departmentId)
        {
            var data = await _context.NotificationLogs.Where(x => x.CompanyId == companyId && x.StaffId == 0
            && x.DepartmentId == departmentId && x.IsUnRead == true).ToListAsync();

            return data.Select(_mapper.Map<NotificationLogsDTO>);
        }

        public async Task<IEnumerable<NotificationLogsDTO>> GetStaffNotificationLogsAsync(int companyId, int staffId)
        {
            var data = await _context.NotificationLogs.Where(x => x.CompanyId == companyId && x.StaffId == staffId
            && x.DepartmentId == 0 && x.IsUnRead == true).ToListAsync();

            return data.Select(_mapper.Map<NotificationLogsDTO>);
        }
    }
}