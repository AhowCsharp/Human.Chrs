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
    public class ContractTypeRepository : BaseRepository<ContractTypeList, ContractTypeDTO, int>, IContractTypeRepository
    {
        public ContractTypeRepository(IMapper mapper, HumanChrsContext context) : base(mapper, context)
        {
        }

        public async Task<IEnumerable<ContractTypeDTO>> AllContractTypeAsync()
        {
            var data = await _context.ContractTypeList.ToListAsync();
            return data.Select(_mapper.Map<ContractTypeDTO>);
        }

        public async Task<ContractTypeDTO> GetContractTypeAsync(int type)
        {
            var data = await _context.ContractTypeList.FirstOrDefaultAsync(x => x.ContractType == type);
            return _mapper.Map<ContractTypeDTO>(data);
        }
    }
}