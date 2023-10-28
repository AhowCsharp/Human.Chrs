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
using System.Collections;

namespace Human.Chrs.Infrastructure.Repositories
{
    public class DepartmentRepository : BaseRepository<Department, DepartmentDTO, int>, IDepartmentRepository
    {
        public DepartmentRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<IEnumerable<DepartmentDTO>> GetDepartmentsOfCompanyAsync(int companyId)
        {
            var data = await _context.Department.Where(x => x.CompanyId == companyId).ToListAsync();

            return data.Select(_mapper.Map<DepartmentDTO>);
        }
    }
}