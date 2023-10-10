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

        public async Task DeleteAllEventsWithMeetIdAsync(int meetId)
        {
            // Find all events with the given meetId
            var eventsToDelete = _context.EventLogs.Where(x => x.MeetId == meetId);

            // Remove the events from the context
            _context.EventLogs.RemoveRange(eventsToDelete);

            // Commit changes to the database
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<EventLogsDTO>> GetCompanyPartimeEventLogsAsync(int companyId)
        {
            var data = await _context.EventLogs.Where(x => x.CompanyId == companyId && x.LevelStatus == 3).ToListAsync();

            return data.Select(_mapper.Map<EventLogsDTO>);
        }

        public async Task AddManyEventLogsAsync(List<EventLogsDTO> dtos)
        {
            var entities = dtos.Select(_mapper.Map<EventLogs>).ToList();

            _context.EventLogs.AddRange(entities);

             await _context.SaveChangesAsync();
        }


        public async Task<bool> RemoveEventLogsAsync(IEnumerable<EventLogsDTO> logsToDelete, int companyId)
        {
            // 获取要删除的日志的ID列表
            var idsToDelete = logsToDelete.Select(log => log.id).ToList();

            // 根据ID列表查询数据库中的相应日志
            var logsInDb = await _context.EventLogs
                                .Where(x => idsToDelete.Contains(x.Id) && x.CompanyId == companyId)
                                .ToListAsync();

            // 从_context中移除这些日志
            _context.EventLogs.RemoveRange(logsInDb);

            // 保存更改到数据库
            return await _context.SaveChangesAsync() > 0;
        }



    }
}