using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.ViewModel.Request
{
    public class MeetRequest
    {
        public int MeetType { get; set; }
        public int? StaffId { get; set; }

        public int? DepartmentId { get; set; }
        public string Title { get; set; }
        public DateTime EventStartDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
        public string Detail { get; set; }

        public int LevelStatus { get; set; }
    }
}