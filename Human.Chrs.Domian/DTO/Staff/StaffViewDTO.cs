using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class StaffViewDTO
    {
        public bool IsOverLocation { get; set; }

        public bool IsCheckIn { get; set; }
        public bool IsCheckOut { get; set; }
        public string CheckInRange { get; set; }

        public string CheckOutRange { get; set; }

        public string AfternoonRange { get; set; }

        public string CompanyName { get; set; }

        public DateTime? ChekinTime { get; set; }

        public DateTime? ChekOutTime { get; set; }

        public IEnumerable<VacationLogDTO> VacationLogDTOs { get; set; }

        public int CheckInStartHour { get; set; }

        public int CheckInStartMinute { get; set; }

        public int CheckInEndHour { get; set; }

        public int CheckInEndMinute { get; set; }

        public int CheckOutStartHour { get; set; }

        public int CheckOutStartMinute { get; set; }

        public int CheckOutEndHour { get; set; }

        public int CheckOutEndMinute { get; set; }

        public string AvatarUrl { get; set; }

        public string Language { get; set; }
    }
}