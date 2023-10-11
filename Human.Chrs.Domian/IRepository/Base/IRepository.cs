using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.IRepository.Base
{
    public interface IRepository<TDTO, TIdentity>
        where TDTO : IDTO
    {
        Task<TDTO> UpdateAsync(TDTO dto);

        Task<TDTO> GetAsync(TIdentity id);

        Task DeleteAsync(TIdentity id);

        Task<IEnumerable<TDTO>> UpdateAsync(IEnumerable<TDTO> dtos);

        Task<TDTO> InsertAsync(TDTO dto);

        Task<IEnumerable<TDTO>> InsertRangeAsync(IEnumerable<TDTO> dtos);

        Task<List<TDTO>> UpdateRangeAsync(List<TDTO> dtos);
    }
}