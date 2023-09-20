using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class DepartmentDTO : IDTO
    {
        public int id { get; set; }

        public int CompanyId { get; set; }

        public int CompanyRuleId { get; set; }

        public string DepartmentName { get; set; }
    }
}