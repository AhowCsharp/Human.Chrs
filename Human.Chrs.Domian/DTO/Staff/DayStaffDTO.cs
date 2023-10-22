using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class DayStaffDTO
    {
        public int StaffId { get; set; }

        public string StaffNo { get; set; }

        public int CompanyId { get; set; }

        public string Department { get; set; }

        public string LevelPosition { get; set; }

        public string WorkLocation { get; set; }

        public int Status { get; set; }

        public string StaffName { get; set; }

        public int? DaySalary { get; set; } // 日薪

        public string WorkDays { get; set; } //工作幾天

        public string Month { get; set; } //月份

        public string LateOrEarlyDays { get; set; }

        public string OutLocationDays { get; set; }

        public int TotalDaysSalary { get; set; }

        public int OverTimeSalary { get; set; }

        public int OverTimeHours { get; set; }
    }
}