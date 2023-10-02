using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class SalarySettingDTO : IDTO
    {
        public int id { get; set; }

        public int StaffId { get; set; }

        public int CompanyId { get; set; }

        public int BasicSalary { get; set; }

        public int FullCheckInMoney { get; set; }

        public int? OtherPercent { get; set; }

        public DateTime EditDate { get; set; }

        public string Editor { get; set; }

        public DateTime CreateDate { get; set; }

        public string Creator { get; set; }

        public string StaffName { get; set; }

        public string StaffNo { get; set; }
    }
}