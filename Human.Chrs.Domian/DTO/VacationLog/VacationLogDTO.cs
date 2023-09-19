using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class VacationLogDTO : IDTO
    {
        public int id { get; set; }

        public int StaffId { get; set; }

        public int CompanyId { get; set; }

        public int VacationType { get; set; }

        public DateTime ApplyDate { get; set; }

        public DateTime ActualStartDate { get; set; }

        public DateTime ActualEndDate { get; set; }

        public int Hours { get; set; }

        public int IsPass { get; set; }

        public string ApproverName { get; set; }

        public int? ApproverId { get; set; }

        public DateTime? AuditDate { get; set; }

        public string Reason { get; set; }

        public string VacationTypeName { get; set; }
    }
}