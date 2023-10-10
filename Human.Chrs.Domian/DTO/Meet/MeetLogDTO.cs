using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class MeetLogDTO : IDTO
    {
        public int id { get; set; }

        public int CompanyId { get; set; }

        public int? StaffId { get; set; }

        public int? DepartmentId { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string Creator { get; set; }

        public DateTime? CreateDate { get; set; }

    }
}