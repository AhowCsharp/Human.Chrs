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

namespace Human.Chrs.Infrastructure.Repositories
{
    public class ShiftWorkListRepository : BaseRepository<ShiftWorkList, ShiftWorkListDTO, int>, IShiftWorkListRepository
    {
        public ShiftWorkListRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<IEnumerable<ShiftWorkListDTO>> GetShiftWorkListAsync(int companyId)
        {
            var data = await _context.ShiftWorkList.Where(x => x.CompanyId == companyId).OrderBy(x => x.StartTime).ToListAsync();

            return data.Select(_mapper.Map<ShiftWorkListDTO>);
        }
    }
}