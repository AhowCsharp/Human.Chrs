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
    public class CompanyRepository : BaseRepository<Company, CompanyDTO, int>, ICompanyRepository
    {
        public CompanyRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }
        public async Task<bool> IsAvailableCompanyAsync(int companyId)
        {
            return await _context.Company.AnyAsync(x => x.Id == companyId);
        }
    }
}