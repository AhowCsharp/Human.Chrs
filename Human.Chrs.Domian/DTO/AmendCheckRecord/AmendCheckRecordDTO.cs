using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.Domain.DTO
{
    public class AmendCheckRecordDTO : IDTO
    {
        public int StaffId { get; set; }

        public int CompanyId { get; set; }

        public DateTime CheckDate { get; set; }

        public DateTime CheckTime { get; set; }

        public int CheckType { get; set; }

        public string Reason { get; set; }

        public int IsValidate { get; set; }

        public string Inspector { get; set; }

        public DateTime? ValidateDate { get; set; }

        public int id { get; set; }

        public string Applicant { get; set; }

        public DateTime ApplicationDate { get; set; }
    }
}