using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class OverTimeLogDTO : IDTO
    {
        public int id { get; set; }

        public DateTime OvertimeDate { get; set; }

        public int OverHours { get; set; }

        public int CompanyId { get; set; }

        public int StaffId { get; set; }

        public int IsValidate { get; set; }

        public string Inspector { get; set; }

        public int? InspectorId { get; set; }

        public DateTime? ValidateDate { get; set; }

        public string Reason { get; set; }
    }
}