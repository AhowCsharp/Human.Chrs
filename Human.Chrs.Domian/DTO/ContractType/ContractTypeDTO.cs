using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class ContractTypeDTO : IDTO
    {
        public int id { get; set; }

        public int ContractType { get; set; }

        public int AdminLimit { get; set; }

        public int StaffLimit { get; set; }
    }
}