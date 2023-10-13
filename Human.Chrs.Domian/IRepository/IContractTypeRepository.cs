using Human.Chrs.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Human.Chrs.Domain.IRepository.Base;

namespace Human.Chrs.Domain.IRepository
{
    public interface IContractTypeRepository : IRepository<ContractTypeDTO, int>
    {
        Task<IEnumerable<ContractTypeDTO>> AllContractTypeAsync();

        Task<ContractTypeDTO> GetContractTypeAsync(int type);
    }
}