using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class UpdateDepartmentRequest
    {
        public int id { get; set; }
        public string DepartmentName { get; set; }

        public int CompanyId { get; set; }
        public int CompanyRuleId { get; set; }
    }
}