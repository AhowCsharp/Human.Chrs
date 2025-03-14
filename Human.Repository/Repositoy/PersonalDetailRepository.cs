﻿using AutoMapper;
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
    public class PersonalDetailRepository : BaseRepository<PersonalDetail, PersonalDetailDTO, int>, IPersonalDetailRepository
    {
        public PersonalDetailRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<PersonalDetailDTO> GetStaffDetailInfoAsync(int staffId, int companyId)
        {
            var data = await _context.PersonalDetail.SingleOrDefaultAsync(x => x.StaffId == staffId && x.CompanyId == companyId);

            return _mapper.Map<PersonalDetailDTO>(data);
        }
    }
}