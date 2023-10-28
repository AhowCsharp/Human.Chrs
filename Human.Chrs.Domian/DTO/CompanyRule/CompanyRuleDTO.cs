using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class CompanyRuleDTO : IDTO
    {
        public int id { get; set; }

        public int CompanyId { get; set; }

        public int DepartmentId { get; set; }

        public TimeSpan CheckInStartTime { get; set; }

        public TimeSpan CheckInEndTime { get; set; }

        public TimeSpan CheckOutStartTime { get; set; }

        public TimeSpan CheckOutEndTime { get; set; }

        public string DepartmentName { get; set; }

        public string AfternoonTime { get; set; }

        public int NeedWorkMinute { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Creator { get; set; }

        public DateTime? EditDate { get; set; }

        public string Editor { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string WorkAddress { get; set; }

        public int? ParttimeStaffId { get; set; }

        public DateTime? ParttimeDate { get; set; }

        public int? ShiftWorkStaffId { get; set; }

        public DateTime? ShiftWorkDarte { get; set; }
    }
}