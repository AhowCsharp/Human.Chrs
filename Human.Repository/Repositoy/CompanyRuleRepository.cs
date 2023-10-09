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
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace LineTag.Infrastructure.Repositories
{
    public class CompanyRuleRepository : BaseRepository<CompanyRule, CompanyRuleDTO, int>, ICompanyRuleRepository
    {
        public CompanyRuleRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<CompanyRuleDTO> GetParttimeRuleAsync(int companyId, int DepartmentId,int staffId,DateTime start,DateTime end)
        {
            var data = await _context.CompanyRule.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.DepartmentId == DepartmentId 
            && x.ParttimeStaffId == staffId && x.ParttimeDate >= start && x.ParttimeDate <= end);
            return _mapper.Map<CompanyRuleDTO>(data);
        }
        public async Task<CompanyRuleDTO> GetCompanyRuleAsync(int companyId, int DepartmentId)
        {
            var data = await _context.CompanyRule.FirstOrDefaultAsync(x => x.CompanyId == companyId && x.DepartmentId == DepartmentId);
            return _mapper.Map<CompanyRuleDTO>(data);
        }

        public async Task<IEnumerable<CompanyRuleDTO>> GetCompanyRulesAsync(int companyId)
        {
            var data = await _context.CompanyRule.Where(x => x.CompanyId == companyId).ToListAsync();
            return data.Select(_mapper.Map<CompanyRuleDTO>);
        }
    }
}