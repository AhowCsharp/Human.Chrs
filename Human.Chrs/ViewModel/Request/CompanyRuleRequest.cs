using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;
using Human.Chrs.ViewModel.Request;

namespace Human.Chrs.ViewModel.Request
{
    public partial class CompanyRuleRequest
    {
        public int id { get; set; }

        public int CompanyId { get; set; }

        public int DepartmentId { get; set; }

        public TimeSpan CheckInStartTime { get; set; }

        public TimeSpan CheckInEndTime { get; set; }

        public TimeSpan CheckOutStartTime { get; set; }

        public TimeSpan CheckOutEndTime { get; set; }

        public string DepartmentName { get; set; }

        public string? AfternoonTime { get; set; }

        public int NeedWorkMinute { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? Creator { get; set; }

        public DateTime? EditDate { get; set; }

        public string? Editor { get; set; }

        public string WorkAddress { get; set; }
    }
}